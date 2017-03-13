﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFTP.Config
{
	public class TransferConfig : ConfigManager
	{
		public TransferConfig() : base("TransferConfig.cfg") { }

		protected override void InitializeConfig()
		{
			AddConfigTable("Accept_Default_Packet", bool.FalseString);

			AddConfigTable("Accept_Basic_Security_Packet", bool.FalseString);
			AddConfigTable("Accept_Anonymous_Login", bool.FalseString);

			AddConfigTable("Accpet_Expert_Security_Packet", bool.TrueString);
			
		}
	}
}
