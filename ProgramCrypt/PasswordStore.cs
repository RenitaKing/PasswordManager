using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
//built in Splashkit.Json wrapper didn't seem to handle the password entries well

namespace PasswordManager
{
	public class PasswordStore
	{
		private List<PasswordEntry> _entries = new List<PasswordEntry>();
		private readonly string _filePath = "Resources/json/passwords.json";
		private int _nextId = 1;

		// add the encrypt service for this session (after login)
		private EncryptionService? _crypto;
		private string? _master;


		// CONSTRUCTOR
		//  load from file at startup
		// Constructor: no load yet (we need the master first)
		public PasswordStore() { }


		//METHODS:

		// Encryption:  Call this right after successful authentication
		public void ConfigureCrypto(EncryptionService crypto, string masterPassword)
		{
			_crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
			_master = masterPassword ?? throw new ArgumentNullException(nameof(masterPassword));
		}

		//CRUD methods:
		// Add a new entry
		public void AddEntry(string website, string username, string password, string notes, string category)
		{
			//we'll add an id number to the entry, to make it easier to find later...
			var entry = new PasswordEntry(_nextId++, website, username, password, notes, category);
			_entries.Add(entry);
			SaveToFile();  //save any changes as we go, so we don't lose anything.
		}

		// Update existing entry by Id
		public bool UpdateById(int id, string newWebsite, string newUsername, string newPassword, string notes, string category)
		{
			//we'll do the checks to make sure we have values for each item when we call this method
			//even if it supplies the same thing
			var entry = FindById(id);
			if (entry != null)
			{
				entry.Website = newWebsite;
				entry.Username = newUsername;
				entry.Password = newPassword;
				entry.Notes = notes;
				entry.Category = category;

				SaveToFile();
				return true;
			}
			return false;
		}

		// Delete entry by Id
		public bool DeleteById(int id)
		{
			var entry = FindById(id);
			if (entry != null)
			{
				_entries.Remove(entry);
				SaveToFile(); //save again.  Is this too brutale - there is no undo!
				return true;
			}
			return false;
		}

		// Get all entries
		public List<PasswordEntry> GetAll()
		{
			return _entries;
		}

		// Search by Id
		public PasswordEntry? FindById(int id)
		{
			//this will need to find an exact match for the id
			return _entries.Find(e => e.Id == id);
		}

		// Search by website (case-insensitive partial match)
		public List<PasswordEntry> FindByWebsite(string website)
		{
			//we'll search for partial matches
			if (string.IsNullOrWhiteSpace(website))
				return new List<PasswordEntry>();

			return _entries
				.Where(e => !string.IsNullOrWhiteSpace(e.Website) &&
							e.Website.IndexOf(website, StringComparison.OrdinalIgnoreCase) >= 0)
				.ToList();
		}

		//FILE INTERACTIONS:
		// Save entries + nextId to JSON file
		public void SaveToFile()
		{
			EnsureCryptoConfigured();
			// Serialise and encrypt each entry
			var encryptedItems = new List<EncryptedEntry>(_entries.Count);
			foreach (var e in _entries)
			{
				string json = JsonSerializer.Serialize(e);
				string enc = _crypto!.EncryptString(json, _master!);

				encryptedItems.Add(new EncryptedEntry
				{
					Id = e.Id,
					EncryptedData = enc
				});
			}
			var data = new EncryptedFileData
			{
				Entries = encryptedItems,
				NextId = _nextId
			};

			var options = new JsonSerializerOptions
			{
				WriteIndented = true //make it pretty
			};

			string jsonString = JsonSerializer.Serialize(data, options);
			File.WriteAllText(_filePath, jsonString);
		}

		// Load entries from JSON file
		public void LoadFromFile()
		{
			_entries.Clear();
			_nextId = 1;

			if (!File.Exists(_filePath))
				return;

			string jsonString = File.ReadAllText(_filePath);

			if (string.IsNullOrWhiteSpace(jsonString))
				return;
			// Try new encrypted format first
			if (TryLoadEncrypted(jsonString)) return;


			Console.WriteLine("Unrecognized password file format.");
		}


		//EXTRA CLASSES TO HELP THE PASSWORD STORE
		// This is a helper class to serialise the password entries into the JSON format
		private class PasswordFileData
		{
			public List<PasswordEntry> Entries { get; set; } = new();
			public int NextId { get; set; }
		}

		// Adding a class to store the whole record as encrypted
		private class EncryptedFileData
		{
			public List<EncryptedEntry> Entries { get; set; } = new();
			public int NextId { get; set; }
		}


		private class EncryptedEntry
		{
			public int Id { get; set; }
			public string EncryptedData { get; set; } = ""; // Base64 of AES-GCM blob

		}


		private bool TryLoadEncrypted(string jsonString)
		{
			try
			{
				var data = JsonSerializer.Deserialize<EncryptedFileData>(jsonString);
				if (data?.Entries == null) return false;

				EnsureCryptoConfigured();

				int maxId = 0;
				foreach (var item in data.Entries)
				{
					string plain = _crypto!.DecryptString(item.EncryptedData, _master!);
					var entry = JsonSerializer.Deserialize<PasswordEntry>(plain);
					if (entry == null) continue;

					_entries.Add(entry);
					if (entry.Id > maxId) maxId = entry.Id;
				}

				_nextId = data.NextId > 0 ? data.NextId : (maxId + 1);
				return true;
			}
			catch
			{
				return false; // not encrypted format or wrong shape
			}
		}


		private void EnsureCryptoConfigured()
		{
			if (_crypto == null || string.IsNullOrEmpty(_master))
				throw new InvalidOperationException("Encryption not configured. Call ConfigureCrypto(...) after login.");
		}

		public void ReencryptAll(EncryptionService crypto, string oldMaster, string newMaster)
		{
			if (crypto == null) throw new ArgumentNullException(nameof(crypto));
			if (string.IsNullOrWhiteSpace(oldMaster)) throw new ArgumentNullException(nameof(oldMaster));
			if (string.IsNullOrWhiteSpace(newMaster)) throw new ArgumentNullException(nameof(newMaster));

			// Load existing file using old master
			if (!File.Exists(_filePath))
				return; // Nothing to re-encrypt

			string jsonString = File.ReadAllText(_filePath);
			if (string.IsNullOrWhiteSpace(jsonString))
				return;

			var data = JsonSerializer.Deserialize<EncryptedFileData>(jsonString);
			if (data?.Entries == null) return;

			var plainEntries = new List<PasswordEntry>();

			try
			{
				foreach (var item in data.Entries)
				{
					string plain = crypto.DecryptString(item.EncryptedData, oldMaster);
					var entry = JsonSerializer.Deserialize<PasswordEntry>(plain);
					if (entry != null) plainEntries.Add(entry);
				}
			}
			catch
			{
				throw new Exception("Failed to decrypt entries with old master password.");
			}

			// Now reconfigure with new master
			_crypto = crypto;
			_master = newMaster;

			_entries = plainEntries;
			_nextId = data.NextId > 0 ? data.NextId : (_entries.Count > 0 ? _entries.Max(e => e.Id) + 1 : 1);

			// Save everything under new key
			SaveToFile();
		}

	}
}