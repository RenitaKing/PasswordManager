namespace PasswordManager
{

	public class PasswordEntry
	{
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";
		public string Website { get; set; } = "";
		public int Id{ get; set; }
		public string Notes{ get; set; } = "";
		public string Category { get; set; } = "";

		//CONSTRUCTORS
		//make a blank constructor, in case we need it
		public PasswordEntry() { }

		//then our proper one...
		public PasswordEntry(int id, string website, string username, string password, string notes, string category)
		{
			Id = id;
			Website = website;
			Username = username;
			Password = password;
			Notes = notes;
			Category = category;
		}

		//METHODS
		//Printing out method, to show the elements to the console.
		public override string ToString()
		{
			return $"(ID: {Id}) *{Category}* [{Website}] {Username} / {Password} - {Notes}";
		}
	}
}
