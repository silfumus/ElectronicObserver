﻿using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_req_kousyou
{

	public class createitem : APIBase
	{

		private int[] materials;

		public createitem()
			: base()
		{

			materials = new int[4];
		}

		public override void OnRequestReceived(Dictionary<string, string> data)
		{

			for (int i = 0; i < 4; i++)
			{
				materials[i] = int.Parse(data["api_item" + (i + 1)]);
			}

			base.OnRequestReceived(data);
		}

		public override void OnResponseReceived(dynamic data)
		{

			KCDatabase db = KCDatabase.Instance;

			//装備の追加　データが不十分のため、自力で構築しなければならない
			if ((int)data.api_create_flag != 0)
			{
				var eq = new EquipmentData();
				eq.LoadFromResponse(APIName, data.api_slot_item);
				db.Equipments.Add(eq);
			}

			db.Material.LoadFromResponse(APIName, data.api_material);

			//logging
			if (Utility.Configuration.Config.Log.ShowSpoiler)
			{
				if ((int)data.api_create_flag != 0)
				{

					int eqid = (int)data.api_slot_item.api_slotitem_id;

					Utility.Logger.Add(2, string.Format(LoggerRes.CreatedItem,
						db.MasterEquipments[eqid].CategoryTypeInstance.Name,
						db.MasterEquipments[eqid].Name,
						materials[0], materials[1], materials[2], materials[3],
						db.Fleet[1].MembersInstance[0].NameWithLevel));
				}
				else
				{
					Utility.Logger.Add(2, string.Format(LoggerRes.ItemCreationFailed,
						materials[0], materials[1], materials[2], materials[3],
						db.Fleet[1].MembersInstance[0].NameWithLevel));
				}
			}

			base.OnResponseReceived((object)data);
		}

		public override bool IsRequestSupported => true;
		public override bool IsResponseSupported => true;

		public override string APIName => "api_req_kousyou/createitem";
	}


}
