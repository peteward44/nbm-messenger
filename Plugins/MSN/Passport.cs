using System;

using Regex = System.Text.RegularExpressions;
using IO = System.IO;
using Net = System.Net;

// 10/10/2003

/// <summary>
/// Handles MS passport authentication.
/// </summary>
public class Passport
{
	private Passport()
	{
	}


	public static string GetLoginAddress(string nexusUri)
	{
		Net.WebClient webClient = new Net.WebClient();

		// open connection to Nexus URL, then grab the headers (it has an empty body)
		IO.Stream strm = webClient.OpenRead(nexusUri);
		string passportUrls = webClient.ResponseHeaders["PassportURLs"];
		strm.Close();

		// get log in part of the http header we are looking at
		Regex.Regex regex = new Regex.Regex(@"DALogin=(?<loginURL>[^\,]+)", Regex.RegexOptions.Compiled);
		Regex.Match match = regex.Match(passportUrls);

		if (!match.Success)
			throw new Exception();

		// put https on front if it isnt there
		string loginUrl = match.Groups["loginURL"].Value.ToLower();
		if (!loginUrl.StartsWith("https://"))
			loginUrl = "https://" + loginUrl;

		// add port number after the host name
		int index = loginUrl.IndexOf("/", 9);
		loginUrl = loginUrl.Substring(0, index) + ":443" + loginUrl.Substring(index);

		return loginUrl;
	}


	public static string Login(string loginUri, string challenge, string username, string password)
	{
		// url-encode username and password
		string urlUsername = uriQuote(username);
		string urlPassword = uriQuote(password);

		Net.WebClient webClient = new Net.WebClient();

		webClient.Headers.Add("Authorization",
			@"Passport1.4 OrgVerb=GET,OrgURL=http%3A%2F%2Fmessenger%2Emsn%2Ecom," + 
			@"sign-in=" + urlUsername + @",pwd=" + urlPassword + @"," + challenge);

		IO.Stream strm = webClient.OpenRead(loginUri);
		strm.Close();

		string authInfo = webClient.ResponseHeaders["Authentication-Info"];

		if (authInfo == null || authInfo == string.Empty || authInfo == "")
		{
			throw new Exception(); // authorization failed
		}
		else
		{
			Regex.Regex regex = new Regex.Regex(@"from-PP='(?<ticket>t=[^\']+)", Regex.RegexOptions.Compiled);
			Regex.Match match = regex.Match(authInfo);

			if (match.Success)
			{
				return match.Groups["ticket"].Value; // success
			}
			else
			{
				// ok, need to redirect then.
				string newUri = webClient.ResponseHeaders["Location"];
				if (newUri == null || authInfo == string.Empty || authInfo == "")
					throw new Exception(); // shouldn't happen

				// start login procedure with new URI.
				return Login(newUri, challenge, username, password);
			}
		}
	}


	private static string uriQuote(string str)
	{
		string newStr = string.Empty;

		for (int i=0; i<str.Length; ++i)
		{
			char c = str[i];

			Regex.Regex regex = new Regex.Regex(@"\w{1}", Regex.RegexOptions.Compiled);
			Regex.Match match = regex.Match(new String(c, 1));

			if (!match.Success)
				newStr += Uri.HexEscape(c);
			else
				newStr += c;
		}

		return newStr;
	}
}

