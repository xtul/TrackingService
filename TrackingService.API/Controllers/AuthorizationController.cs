using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrackingService.API.Database;
using TrackingService.Model.Objects;
using TrackingService.Model.Objects.DTOs;

namespace TrackingService.API.Controllers {
	[Route("api/auth")]
	[ApiController]
	public class AuthManagementController : ControllerBase {
		private readonly UserManager<TrackingUser> _userManager;
		private readonly JwtConfig _jwtConfig;
		private readonly TokenValidationParameters _tokenParams;
		private readonly TrackingDbContext _context;
		private readonly ILogger<AuthManagementController> _logger;

		public AuthManagementController(
			UserManager<TrackingUser> userManager,
			JwtConfig jwtConfig,
			TokenValidationParameters tokenParams,
			TrackingDbContext context,
			ILogger<AuthManagementController> logger) {
			_userManager = userManager;
			_jwtConfig = jwtConfig;
			_tokenParams = tokenParams;
			_context = context;
			_logger = logger;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationDto user) {
			if (!ModelState.IsValid) {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>() {
						"Invalid payload."
					}
				});
			}

			var userExists = await _userManager.FindByEmailAsync(user.Email);
			if (userExists is not null) {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>(){
							"Email already exists."
						}
				});
			}

			var newUser = new TrackingUser() { Email = user.Email, UserName = user.Name };
			var createResult = await _userManager.CreateAsync(newUser, user.Password);
			if (createResult.Succeeded) {
				var jwtToken = await GenerateJwtTokenAsync(newUser);

				return Ok(jwtToken);
			}

			return BadRequest(new AuthResult() {
				Success = false,
				Errors = createResult.Errors.Select(x => x.Description).ToList()
			});
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto user) {
			if (!ModelState.IsValid) {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>() {
						"Invalid payload."
					}
				});
			}

			var existingUser = await _userManager.FindByEmailAsync(user.Email);
			if (existingUser is null) {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>(){
						"Invalid request."
					}
				});
			}

			var isPasswordCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);
			if (isPasswordCorrect) {
				var jwtToken = GenerateJwtTokenAsync(existingUser);

				return Ok(jwtToken);
			} else {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>(){
						"Invalid request."
					}
				});

			}
		}

		[HttpPost]
		[Route("refresh")]
		public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest) {
			if (ModelState.IsValid) {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>() {
						"Invalid payload."
					}
				});
			}

			var res = await VerifyToken(tokenRequest);

			if (res is null) {
				return BadRequest(new AuthResult() {
					Success = false,
					Errors = new List<string>() {
						"Invalid token."
					}
				});
			}

			return Ok(res);
		}

		private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequest) {
			var jwtTokenHandler = new JwtSecurityTokenHandler();

			try {
				// check if token is valid (against defined token parameters)
				var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenParams, out var validatedToken);
				if (validatedToken is JwtSecurityToken jwtSecurityToken) {
					var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

					if (!result) {
						return null;
					}
				}

				var expirationDateTimestamp = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
				var expirationDate = UnixTimeStampToDateTime(expirationDateTimestamp);

				if (expirationDate > DateTime.UtcNow) {
					return new AuthResult() {
						Success = false,
						Errors = new List<string>() { 
							"Token has not expired yet."
						}
					};
				}

				var storedRefreshToken = _context.RefreshTokens.AsNoTracking().FirstOrDefault(x => x.Token == tokenRequest.RefreshToken);
				if (storedRefreshToken == null) {
					return new AuthResult() {
						Success = false,
						Errors = new List<string>() { 
							"No such refresh token exists."
						}
					};
				}

				if (DateTime.UtcNow > storedRefreshToken.ExpiryDate) {
					return new AuthResult() {
						Success = false,
						Errors = new List<string>() {
							"Token expired."
						}
					};
				}

				if (storedRefreshToken.IsUsed) {
					return new AuthResult() {
						Success = false,
						Errors = new List<string>() {
							"Token is in use."
						}
					};
				}

				if (storedRefreshToken.IsRevoked) {
					return new AuthResult() {
						Success = false,
						Errors = new List<string>() {
							"Token has been revoked."
						}
					};
				}

				var jwtId = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
				if (storedRefreshToken.JwtId != jwtId) {
					return new AuthResult() {
						Success = false,
						Errors = new List<string>() {
							"Provided token doesn't match saved token."
						}
					};
				}

				storedRefreshToken.IsUsed = true;
				_context.RefreshTokens.Update(storedRefreshToken);
				await _context.SaveChangesAsync();

				var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
				return await GenerateJwtTokenAsync(dbUser);
			} catch (Exception ex) {
				_logger.LogError(ex.ToString());
				return null;
			}
		}

		private static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		private async Task<AuthResult> GenerateJwtTokenAsync(TrackingUser user) {
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(new[] {
					new Claim("Id", user.Id.ToString()),
					new Claim(JwtRegisteredClaimNames.Sub, user.Email),
					new Claim(JwtRegisteredClaimNames.Email, user.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				}),
				Expires = DateTime.UtcNow.AddSeconds(_jwtConfig.SecondsLifespan),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
			};

			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			var jwtToken = jwtTokenHandler.WriteToken(token);

			var refreshToken = new RefreshToken() {
				JwtId = token.Id,
				IsUsed = false,
				UserId = user.Id,
				AddedDate = DateTime.UtcNow,
				ExpiryDate = DateTime.Now.AddYears(1),
				IsRevoked = false,
				Token = RandomString() + Guid.NewGuid()
			};

			await _context.RefreshTokens.AddAsync(refreshToken);
			await _context.SaveChangesAsync();

			return new AuthResult() {
				Success = true,
				Token = jwtToken,
				RefreshToken = refreshToken.Token
			};
		}

		private static string RandomString(int length = 25) {
			var random = new Random();
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
