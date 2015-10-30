using System;

using Regex = System.Text.RegularExpressions;

// 29/4/03

public class Message
{
	private string message, header, code, body = string.Empty, arguments = string.Empty;
	private int transactionID;


	public string RawMessage
	{
		get { return message; }
	}

	public string Header
	{
		get { return header; }
	}

	public string Body
	{
		get { return body; }
		set
		{
			body = value;
			message += body;
		}
	}

	public string Code
	{
		get { return code; }
	}

	public string Arguments
	{
		get { return arguments; }
	}

	public int TransactionID
	{
		get { return transactionID; }
	}


	public Message(string message)
	{
		this.message = message;

		Regex.Regex regex = new Regex.Regex(@"(?<code>[\w\d]+)\s*(?<transID>\d*)\s*(?<arguments>[^\r\n]*)([\r\n]*)(?<body>.*$)", Regex.RegexOptions.Singleline);
	  Regex.Match match = regex.Match(message);

		if (match.Success)
		{
			this.code = match.Groups["code"].Value;
			this.transactionID = (match.Groups["transID"].Success && match.Groups["transID"].Value != "") ? int.Parse(match.Groups["transID"].Value) : -1;

			this.arguments = match.Groups["arguments"].Success ? match.Groups["arguments"].Value : string.Empty;
			this.body = match.Groups["body"].Success ? match.Groups["body"].Value : string.Empty;

			if (transactionID != -1)
				this.header = code + " " + transactionID + " " + arguments;
			else
				this.header = code + " " + arguments;
		}
	}


	private static int nextTransactionID = 0;


	private Message()
	{
	}


	public static Message ConstructMessage(string code)
	{
		return ConstructMessage(code, string.Empty);
	}


	public static Message ConstructMessage(string code, string arguments)
	{
		return ConstructMessage(code, arguments, string.Empty, true);
	}


	public static Message ConstructMessage(string code, string arguments, string body, bool useTransactionID)
	{
		Message mess = new Message();

		mess.transactionID = (useTransactionID ? nextTransactionID++ : -1);

		mess.code = code;
		mess.arguments = arguments;
		mess.body = body;
		mess.header = code + (useTransactionID ? " " + mess.transactionID : "") + (arguments != string.Empty ? " " + arguments : "");

		mess.message = mess.header;

		if (body != string.Empty)
		{
			mess.message += "\r\n" + body;
		}

		return mess;
	}
}

