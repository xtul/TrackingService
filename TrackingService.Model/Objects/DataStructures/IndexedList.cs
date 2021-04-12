using System;
using System.Collections.Generic;
using System.Linq;

namespace TrackingService.Model.Objects.DataStructures {
	/// <summary>
	/// A class wrapping Dictionary to split a List by key to speed up retrieval.
	/// </summary>
	/// <remarks>
	/// For example, if you store a lot of <see cref="Position"/>s in a <see cref="List{T}"/> and use LINQ to
	/// find all positions by IMEI, performance will plummet. <see cref="IndexedList{T}"/> allows you to ask 
	/// for a specific key and then optionally make LINQ query on underylying <see cref="List{T}"/>.
	/// </remarks>
	/// <typeparam name="T">A type to store in lists.</typeparam>
	public class IndexedList<T> where T : TrackingEntity {
		private readonly Dictionary<string, List<T>> _dict;

		public IndexedList() {
			_dict = new();
		}

		/// <summary>
		/// Finds a single <typeparamref name="T"/>. If <paramref name="func"/> is null, returns first.
		/// </summary>
		/// <remarks>
		/// Returns <see cref="null"/> if not found.
		/// </remarks>
		public T FindSingle(string key, Func<T, bool> func = null) {
			if (func is null) {
				return _dict[key].FirstOrDefault();
			}


			return _dict[key].SingleOrDefault(func);
		}

		public List<T> FindMany(string key, Func<T, bool> func) => _dict[key]?.Where(func).ToList() ?? null;

		public List<T> GetAll(string key) => _dict[key] ?? null;

		/// <returns>If removal was successful.</returns>
		public bool RemoveAll(string key) {
			if (!_dict.ContainsKey(key)) {
				return false;
			}

			return _dict.Remove(key);
		}

		/// <summary>
		/// Removes all <typeparamref name="T"/>s under <typeparamref name="key"/>. If <paramref name="func"/> is null, removes entire key.
		/// </summary>
		/// <returns>If removal was successful.</returns>
		public bool Remove(string key, Predicate<T> func) {
			if (!_dict.ContainsKey(key)) {
				return false;
			}

			if (func is null) {
				return _dict.Remove(key);
			}

			return _dict[key].RemoveAll(func) > 0;
		}

		/// <summary>
		/// Removes everything from <see cref="IndexedList{T}"/>.
		/// </summary>
		public void Clear() => _dict.Clear();

		public void Add(string key, T value) {
			EnsureKeyExists(key);

			_dict[key].Add(value);
		}

		public void AddMany(string key, IEnumerable<T> toAdd) {
			EnsureKeyExists(key);

			_dict[key].AddRange(toAdd);
		}
		public bool Replace(string key, T oldItem, T newItem) {
			var itemToUpdate = _dict[key].Find(x => x.Equals(oldItem));

			if (itemToUpdate is null) {
				return false;
			}

			itemToUpdate = newItem;
			return true;
		}


		/// <summary>
		/// Adds many <typeparamref name="T"/>s, each under a key stored in <paramref name="propertyAsKey"/> property.
		/// </summary>
		public void AddManyWithImeiKey(IEnumerable<T> toAdd) {
			var keysToAdd = toAdd.Select(x => x.Imei);

			foreach (var key in keysToAdd) {
				AddMany(key.ToString(), toAdd.Where(x => x.Imei == key));
			}
		}

		public void AddManyWithIdKey(IEnumerable<T> toAdd) {
			var keysToAdd = toAdd.Select(x => x.Id);
		
			foreach (var key in keysToAdd) {
				AddMany(key.ToString(), toAdd.Where(x => x.Id == key));
			}
		}

		/// <summary>
		/// Counts the elements in all lists.
		/// </summary>
		/// <returns></returns>
		public int Count() {
			int count = 0;

			foreach (var list in _dict.Values) {
				count += list.Count;
			}

			return count;
		}

		/// <summary>
		/// Returns all elements as a single, unsorted list.
		/// </summary>
		/// <returns></returns>
		public List<T> Flattened() {
			List<T> result = new();

			foreach (var list in _dict.Values) {
				result.AddRange(list);
			}

			return result;
		}

		private void EnsureKeyExists(string key) {
			if (!_dict.ContainsKey(key)) {
				_dict.Add(key, new List<T>());
			}
		}
	}
}
