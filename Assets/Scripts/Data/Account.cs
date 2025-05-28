using System.IO;
using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public enum HairStyle
{
	STRAIGHT,
	WAVY
}

[System.Serializable]
public class Account
{
	/// <summary>The account's username.</summary>
	[SerializeField] private string username;
	
	/// <summary>The account's password.</summary>
	[SerializeField] private string password;

	/// <summary>The account's password.</summary>
	[SerializeField] private Color bodyColor;

	/// <summary>The account's password.</summary>
	[SerializeField] private Color hairColor;

	/// <summary>The account's password.</summary>
	[SerializeField] private HairStyle hairStyle;

	public Account(string username, string password, Color bodyColor, Color hairColor, HairStyle style)
	{
		this.username = username;
		this.password = password;
		this.bodyColor = bodyColor;
		this.hairColor = hairColor;
		this.hairStyle = style;
	}

	/// <summary>
	/// Get the account's username.
	/// </summary>
	/// <returns>The account's username.</returns>
	public string GetUsername()
	{
		return this.username;
	}

	/// <summary>
	/// Get the account's password.
	/// </summary>
	/// <returns>The account's password.</returns>
	public string GetPassword()
	{
		return this.password;
	}

	public Color GetBodyColor()
	{
		return this.bodyColor;
	}

	public Color GetHairColor()
	{
		return this.hairColor;
	}
	
	public HairStyle GetHairStyle()
	{
		return this.hairStyle;
	}

	/// <summary>
	/// Save the account to a json file. The file will be named after the account's username for easy lookup.
	/// </summary>
	public void SaveToFile()
	{
		if (!Directory.Exists(GetAccountsDirectory()))
		{
			Directory.CreateDirectory(GetAccountsDirectory());
		}

		string accountJson = JsonUtility.ToJson(this);
		File.WriteAllText(GetAccountsDirectory() + '/' + GetAccountFileName(this.username), accountJson);
	}

	/// <summary>
	/// Get the file path for an account's json.
	/// </summary>
	/// <param name="username">The name of the account.</param>
	/// <returns>A string representation of the json file's path in the system.</returns>
	public static string GetAccountFileName(string username)
	{
		return username + ".json";
	}

	public static string GetAccountsDirectory()
	{
		return Application.persistentDataPath + "/accounts";
	}

	/// <summary>
	/// Check if an account already has a json file associated with it.
	/// </summary>
	/// <param name="username">The name of the account.</param>
	/// <returns>True if a file was found, false otherwise.</returns>
	public static bool HasFile(string username)
	{
		return File.Exists(GetAccountsDirectory() + '/' + GetAccountFileName(username));
	}

	/// <summary>
	/// Load an account from file.
	/// </summary>
	/// <param name="username">The name of the account.</param>
	/// <returns></returns>
	public static Account LoadFromFile(string username)
	{
		if(!HasFile(username))
		{
			return null;
		}

		string content = File.ReadAllText(GetAccountsDirectory() + '/' + GetAccountFileName(username));
		return JsonUtility.FromJson<Account>(content);
	}

	/// <summary>
	/// Verify that a username is valid based on the system's specifications.
	/// </summary>
	/// <param name="username">The username to verify.</param>
	/// <returns>True if the username is valid, false otherwise.</returns>
	public static string VerifyUsername(string username)
	{
		if(username == null)
		{
			return "Null is not a valid username.";
		}

		if(username.Length <= 0)
		{
			return "Please enter a username.";
		}

		if(username.Length > 20)
		{
			return "Please enter a shorter username.\n(Max of 20 characters)";
		}

		if(!Regex.IsMatch(username, "^\\w+$"))
		{
			return "The username contains illegal characters.\nValid characters: a-z, A-Z, 0-9, _";
		}

		return "";
	}

	/// <summary>
	/// Verify that a password is valid based on the system's specifications.
	/// </summary>
	/// <param name="password">The password to verify.</param>
	/// <returns>True if the password is valid, false otherwise.</returns>
	public static string VerifyPassword(string password)
	{
		if(password == null)
		{
			return "Null is not a valid password.";
		}

		if(password.Length <= 0)
		{
			return "Please enter a password.";
		}

		if(password.Length < 8)
		{
			return "Please enter a longer password.\n(Min of 8 characters)";
		}

		if(password.Length > 20)
		{
			return "Please enter a shorter password.\n(Max of 20 characters)";
		}

		if(!Regex.IsMatch(password, "^[\\w!#$%&?]+$"))
		{
			return "The password contains illegal characters.\nValid characters: a-z, A-Z, 0-9, _!#$%&?";
		}

		return "";
	}
}