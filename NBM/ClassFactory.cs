using System;
using System.Collections;
using System.Reflection;
using NBM.Plugin;

// 2/6/03

namespace NBM
{
	/// <summary>
	/// Creates class instances for the plugin architecture.
	/// </summary>
	public class ClassFactory
	{
		private Assembly assembly;


		/// <summary>
		/// Constructs an instance of ClassFactory
		/// </summary>
		/// <param name="fileName">Filename of the assembly to load instances from</param>
		public ClassFactory(string fileName)
		{
			this.assembly = Assembly.LoadFrom(fileName);
		}


		/// <summary>
		/// Creates an instance of the plugin-implemented IProtocol class.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public IProtocol CreateProtocol(ProtocolControl control, ProtocolSettings settings)
		{
			return (IProtocol)this.assembly.CreateInstance(Config.Constants.ProtocolClassName, true,
				BindingFlags.CreateInstance, null,
				new object[] { control, settings }, null, null);
		}


		/// <summary>
		/// Creates an instance of the plugin-implemented IConversation class.
		/// </summary>
		/// <param name="protocol"></param>
		/// <param name="control"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public IConversation CreateConversation(IProtocol protocol, ConversationControl control, ProtocolSettings settings)
		{
			return (IConversation)this.assembly.CreateInstance(Config.Constants.ConversationClassName, true,
				BindingFlags.CreateInstance, null,
				new object[] { protocol, control, settings }, null, null);
		}


		/// <summary>
		/// Creates an instance of the plugin-implemented ProtocolSettings class.
		/// </summary>
		/// <param name="constants"></param>
		/// <param name="storage"></param>
		/// <param name="optionsList"></param>
		/// <returns></returns>
		public ProtocolSettings CreateSettings(IConstants constants, IStorage storage, ArrayList optionsList)
		{
			ProtocolSettings settings = null;

			try
			{
				settings = (ProtocolSettings)this.assembly.CreateInstance(Config.Constants.SettingsClassName, true,
					BindingFlags.CreateInstance, null,
					new object[] { constants, storage, optionsList }, null, null);
			}
			catch
			{}
			finally
			{
				if (settings == null)
					settings = new ProtocolSettings(constants, storage, optionsList);
			}

			return settings;
		}


		/// <summary>
		/// Creates an instance of the plugin-implemented IConstants class.
		/// </summary>
		/// <returns></returns>
		public IConstants CreateConstants()
		{
			return (IConstants)this.assembly.CreateInstance(Config.Constants.ConstantsClassName);
		}
	}
}
