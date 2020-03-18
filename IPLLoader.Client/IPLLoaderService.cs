using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Communications;
using NFive.SDK.Client.Events;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using Addemod.IPLLoader.Shared;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using NFive.SDK.Core.Models;
using System.Linq;
using CitizenFX.Core;
using Configuration = Addemod.IPLLoader.Shared.Configuration;

namespace Addemod.IPLLoader.Client {
	[PublicAPI]
	public class IPLLoaderService : Service {
		private Configuration config;

		public IPLLoaderService(ILogger logger, ITickManager ticks, ICommunicationManager comms, ICommandManager commands, IOverlayManager overlay, User user) : base(logger, ticks, comms, commands, overlay, user) { }

		public override async Task Started() {
			// Request server configuration
			this.config = await this.Comms.Event(IPLLoaderEvents.Configuration).ToServer().Request<Configuration>();

			// Attach a tick handler
			//this.Ticks.On(LoadIPLTick);


			API.LoadMpDlcMaps(); // Required to load heist IPL
			Logger.Info("Loaded DLC maps");

			this.RequestAllIPLS();

			Logger.Info("Requested all IPLs");

			foreach (var ipl in this.config.ipls) {
				this.LoadInterior(ipl.Locations, ipl.InteriorProps);
			}
		}

		private void LoadIPLTick() {
			this.Ticks.Off(LoadIPLTick); // Turn off after first tick!
		}

		private void LoadInterior(List<NFive.SDK.Core.Models.Vector3> locations, List<InteriorProp> interiorProps) {
			foreach (var location in locations) {
				// Try to get the interior from the location specified
				var interiorId = API.GetInteriorAtCoords(location.X, location.Y, location.Z);
				if (API.IsValidInterior(interiorId)) {
					// Load the interior to memory
					API.PinInteriorInMemory(interiorId);

					// Load all props for the interior
					foreach (var interiorProp in interiorProps) {
						API.ActivateInteriorEntitySet(interiorId, interiorProp.Name);

						// Load prop color if it has any
						if (interiorProp.Color.HasValue) {
							API.SetInteriorEntitySetColor(interiorId, interiorProp.Name, interiorProp.Color.Value);
						}
					}

					// Refresh to get all props that were added in the interior
					API.RefreshInterior(interiorId);

					Logger.Info("Loaded interior ID " + interiorId);
				} else {
					Logger.Info("Couldn't find interior at " + location.ToString());
				}
			}
		}

		private void RequestAllIPLS() {
			//Simeon: -47.162, -1115.333, 26.5
			API.RequestIpl("shr_int");

			// Trevor: 1985.481, 3828.768, 32.5
			// Trash or Tidy. Only choose one.
			API.RequestIpl("TrevorsTrailerTrash");
			//API.RequestIpl("TrevorsTrailerTidy");

			// Vangelico Jewelry Store: -637.202, -239.163, 38.1
			API.RequestIpl("post_hiest_unload");

			// Max Renda: -585.825, -282.72, 35.455
			API.RequestIpl("refit_unload");

			// Heist Union Depository: 2.697, -667.017, 16.130
			API.RequestIpl("FINBANK");

			// Morgue: 239.752, -1360.650, 39.534
			API.RequestIpl("Coroner_Int_on");
			API.RequestIpl("coronertrash");

			// Cluckin Bell: -146.384, 6161.5, 30.2
			API.RequestIpl("CS1_02_cf_onmission1");
			API.RequestIpl("CS1_02_cf_onmission2");
			API.RequestIpl("CS1_02_cf_onmission3");
			API.RequestIpl("CS1_02_cf_onmission4");

			// Grapeseed Cow Farm: 2447.9, 4973.4, 47.7
			API.RequestIpl("farm");
			API.RequestIpl("farmint");
			API.RequestIpl("farm_lod");
			API.RequestIpl("farm_props");
			API.RequestIpl("des_farmhouse");

			// FIB lobby: 105.456, -745.484, 44.755
			API.RequestIpl("FIBlobby");

			// Billboard: iFruit
			API.RequestIpl("FruitBB");
			API.RequestIpl("sc1_01_newbill");
			API.RequestIpl("hw1_02_newbill");
			API.RequestIpl("hw1_emissive_newbill");
			API.RequestIpl("sc1_14_newbill");
			API.RequestIpl("dt1_17_newbill");

			// Lester's factory: 716.84, -962.05, 31.59
			API.RequestIpl("id2_14_during_door");
			API.RequestIpl("id2_14_during1");

			// Life Invader lobby: -1047.9, -233.0, 39.0
			API.RequestIpl("facelobby");

			// Tunnels
			API.RequestIpl("v_tunnel_hole");

			// Carwash: 55.7, -1391.3, 30.5
			API.RequestIpl("Carwash_with_spinners");

			// Stadium 'Fame or Shame': -248.492, -2010.509, 34.574
			API.RequestIpl("sp1_10_real_interior");
			API.RequestIpl("sp1_10_real_interior_lod");

			// House in Banham Canyon: -3086.428, 339.252, 6.372
			API.RequestIpl("ch1_02_open");

			// Garage in La Mesa(autoshop);: 970.275, -1826.570, 31.115
			API.RequestIpl("bkr_bi_id1_23_door");

			// Hill Valley church - Grave: -282.464, 2835.845, 55.914
			API.RequestIpl("lr_cs6_08_grave_closed");

			// Lost's trailer park: 49.494, 3744.472, 46.386
			API.RequestIpl("methtrailer_grp1");

			// Lost safehouse: 984.155, -95.366, 74.50
			API.RequestIpl("bkr_bi_hw1_13_int");

			// Raton Canyon river: -1652.83, 4445.28, 2.52
			API.RequestIpl("CanyonRvrShallow");

			// Zancudo Gates(GTAO like);: -1600.301, 2806.731, 18.797
			API.RequestIpl("CS3_07_MPGates");

			// Pillbox hospital: 356.8, -590.1, 43.3
			API.RequestIpl("RC12B_Default");
			// API.RequestIpl("RC12B_Fixed");

			// Josh's house: -1117.163, 303.1, 66.522
			API.RequestIpl("bh1_47_joshhse_unburnt");
			API.RequestIpl("bh1_47_joshhse_unburnt_lod");

			// Zancudo River(need streamed content);: 86.815, 3191.649, 30.463
			API.RequestIpl("cs3_05_water_grp1");
			API.RequestIpl("cs3_05_water_grp1_lod");
			API.RequestIpl("cs3_05_water_grp2");
			API.RequestIpl("cs3_05_water_grp2_lod");

			// Cassidy Creek(need streamed content);: -425.677, 4433.404, 27.325
			API.RequestIpl("canyonriver01");
			API.RequestIpl("canyonriver01_lod");

			// Graffitis
			API.RequestIpl("ch3_rd2_bishopschickengraffiti");// 1861.28, 2402.11, 58.53
			API.RequestIpl("cs5_04_mazebillboardgraffiti");// 2697.32, 3162.18, 58.1
			API.RequestIpl("cs5_roads_ronoilgraffiti");// 2119.12, 3058.21, 53.25

			// Aircraft Carrier(USS Luxington);: 3082.312 - 4717.119 15.262
			API.RequestIpl("hei_carrier");
			API.RequestIpl("hei_carrier_distantlights");
			API.RequestIpl("hei_Carrier_int1");
			API.RequestIpl("hei_Carrier_int2");
			API.RequestIpl("hei_Carrier_int3");
			API.RequestIpl("hei_Carrier_int4");
			API.RequestIpl("hei_Carrier_int5");
			API.RequestIpl("hei_Carrier_int6");
			API.RequestIpl("hei_carrier_lodlights");
			API.RequestIpl("hei_carrier_slod");

			// Galaxy Super Yacht: -2043.974,-1031.582, 11.981
			API.RequestIpl("hei_yacht_heist");
			API.RequestIpl("hei_yacht_heist_Bar");
			API.RequestIpl("hei_yacht_heist_Bedrm");
			API.RequestIpl("hei_yacht_heist_Bridge");
			API.RequestIpl("hei_yacht_heist_DistantLights");
			API.RequestIpl("hei_yacht_heist_enginrm");
			API.RequestIpl("hei_yacht_heist_LODLights");
			API.RequestIpl("hei_yacht_heist_Lounge");

			// Bahama Mamas: -1388, -618.420, 30.820
			//API.RequestIpl("hei_sm_16_interior_v_bahama_milo_");

			// Red Carpet: 300.593, 199.759, 104.378
			//API.RequestIpl("redCarpet");

			// UFO
			// Zancudo: -2052, 3237, 1457
			// Hippie base: 2490.5, 3774.8, 2414
			// Chiliad: 501.53, 5593.86, 796.23
			// API.RequestIpl("ufo");
			// API.RequestIpl("ufo_eye");
			// API.RequestIpl("ufo_lod");

			// Appartments & Offices
			// Some have multiple choices, in that case pick one

			//Arcadius Business Centre: -141.29, -621, 169

			// API.RequestIpl("ex_dt1_02_office_01a");// Old Spice: Warm
			API.RequestIpl("ex_dt1_02_office_01b");// Old Spice: Classical
												   // API.RequestIpl("ex_dt1_02_office_01c");// Old Spice: Vintage

			// API.RequestIpl("ex_dt1_02_office_02a");// Executive: Contrast
			// API.RequestIpl("ex_dt1_02_office_02b");// Executive: Rich
			// API.RequestIpl("ex_dt1_02_office_02c");// Executive: Cool

			// API.RequestIpl("ex_dt1_02_office_03a");// Power Broker: Ice
			// API.RequestIpl("ex_dt1_02_office_03b");// Power Broker: Conservative
			// API.RequestIpl("ex_dt1_02_office_03c");// Power Broker: Polished

			// Maze Bank Building: -75.498, -827.189, 243.386

			// API.RequestIpl("ex_dt1_11_office_01a");// Old Spice: Warm
			API.RequestIpl("ex_dt1_11_office_01b");// Old Spice: Classical
												   // API.RequestIpl("ex_dt1_11_office_01c");// Old Spice: Vintage

			// API.RequestIpl("ex_dt1_11_office_02b");// Executive: Rich
			// API.RequestIpl("ex_dt1_11_office_02c");// Executive: Cool
			// API.RequestIpl("ex_dt1_11_office_02a");// Executive: Contrast

			// API.RequestIpl("ex_dt1_11_office_03a");// Power Broker: Ice
			// API.RequestIpl("ex_dt1_11_office_03b");// Power Broker: Conservative
			// API.RequestIpl("ex_dt1_11_office_03c");// Power Broker: Polished

			// Lom Bank: -1579.756, -565.066, 108.523

			// API.RequestIpl("ex_sm_13_office_01a");// Old Spice: Warm
			API.RequestIpl("ex_sm_13_office_01b");// Old Spice: Classical
												  // API.RequestIpl("ex_sm_13_office_01c");// Old Spice: Vintage
												  // API.RequestIpl("ex_sm_13_office_02a");// Executive: Contrast
												  // API.RequestIpl("ex_sm_13_office_02b");// Executive: Rich
												  // API.RequestIpl("ex_sm_13_office_02c");// Executive: Cool
												  // API.RequestIpl("ex_sm_13_office_03a");// Power Broker: Ice
												  // API.RequestIpl("ex_sm_13_office_03b");// Power Broker: Conservative
												  // API.RequestIpl("ex_sm_13_office_03c");// Power Broker: Polished

			// Maze Bank West: -1392.667, -480.474, 72.042

			// API.RequestIpl("ex_sm_15_office_01a");// Old Spice: Warm
			API.RequestIpl("ex_sm_15_office_01b");// Old Spice: Classical
												  // API.RequestIpl("ex_sm_15_office_01c");// Old Spice: Vintage
												  // API.RequestIpl("ex_sm_15_office_02b");// Executive: Rich
												  // API.RequestIpl("ex_sm_15_office_02c");// Executive: Cool
												  // API.RequestIpl("ex_sm_15_office_02a");// Executive: Contrast
												  // API.RequestIpl("ex_sm_15_office_03a");// Power Broker: Ice
												  // API.RequestIpl("ex_sm_15_office_03b");// Power Broker: Convservative
												  // API.RequestIpl("ex_sm_15_office_03c");// Power Broker: Polished

			// Apartment 1: -786.866, 315.764, 217.638

			API.RequestIpl("apa_v_mp_h_01_a");// Modern
											  // API.RequestIpl("apa_v_mp_h_02_a");// Mody
											  // API.RequestIpl("apa_v_mp_h_03_a");// Vibrant
											  // API.RequestIpl("apa_v_mp_h_04_a");// Sharp
											  // API.RequestIpl("apa_v_mp_h_05_a");// Monochrome
											  // API.RequestIpl("apa_v_mp_h_06_a");// Seductive
											  // API.RequestIpl("apa_v_mp_h_07_a");// Regal
											  // API.RequestIpl("apa_v_mp_h_08_a");// Aqua

			// Apartment 2: -786.956, 315.622, 187.913

			// API.RequestIpl("apa_v_mp_h_01_c");// Modern
			API.RequestIpl("apa_v_mp_h_02_c");// Mody
											  // API.RequestIpl("apa_v_mp_h_03_c");// Vibrant
											  // API.RequestIpl("apa_v_mp_h_04_c");// Sharp
											  // API.RequestIpl("apa_v_mp_h_05_c");// Monochrome
											  // API.RequestIpl("apa_v_mp_h_06_c");// Seductive
											  // API.RequestIpl("apa_v_mp_h_07_c");// Regal
											  // API.RequestIpl("apa_v_mp_h_08_c");// Aqua

			// Apartment 3: -774.012, 342.042, 196.686

			// API.RequestIpl("apa_v_mp_h_01_b");// Modern
			// API.RequestIpl("apa_v_mp_h_02_b");// Mody
			// API.RequestIpl("apa_v_mp_h_03_b");// Vibrant
			API.RequestIpl("apa_v_mp_h_04_b");// Sharp
											  // API.RequestIpl("apa_v_mp_h_05_b");// Monochrome
											  // API.RequestIpl("apa_v_mp_h_06_b");// Seductive
											  // API.RequestIpl("apa_v_mp_h_07_b");// Regal
											  // API.RequestIpl("apa_v_mp_h_08_b");// Aqua

			// Bunkers, Biker clubhouses &Warehouses

			// Clubhouse 1: 1107.04, -3157.399, -37.519
			API.RequestIpl("bkr_biker_interior_placement_interior_0_biker_dlc_int_01_milo");

			// Clubhouse 2: 998.4809, -3164.711, -38.907
			API.RequestIpl("bkr_biker_interior_placement_interior_1_biker_dlc_int_02_milo");

			// Warehouse 1: 1009.5, -3196.6, -39
			API.RequestIpl("bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo");
			API.RequestIpl("bkr_biker_interior_placement_interior_2_biker_dlc_int_ware02_milo");
			API.RequestIpl("bkr_biker_interior_placement_interior_2_biker_dlc_int_ware03_milo");
			API.RequestIpl("bkr_biker_interior_placement_interior_2_biker_dlc_int_ware04_milo");
			API.RequestIpl("bkr_biker_interior_placement_interior_2_biker_dlc_int_ware05_milo");

			// Warehouse 2: 1051.491, -3196.536, -39.148
			API.RequestIpl("bkr_biker_interior_placement_interior_3_biker_dlc_int_ware02_milo");

			// Warehouse 3: 1093.6, -3196.6, -38.998
			API.RequestIpl("bkr_biker_interior_placement_interior_4_biker_dlc_int_ware03_milo");

			// Warehouse 4: 1121.897, -3195.338, -40.4025
			API.RequestIpl("bkr_biker_interior_placement_interior_5_biker_dlc_int_ware04_milo");

			// Warehouse 5: 1165, -3196.6, -39.013
			API.RequestIpl("bkr_biker_interior_placement_interior_6_biker_dlc_int_ware05_milo");

			// Warehouse Small: 1094.988, -3101.776, -39
			API.RequestIpl("ex_exec_warehouse_placement_interior_1_int_warehouse_s_dlc_milo");

			// Warehouse Medium: 1056.486, -3105.724, -39
			API.RequestIpl("ex_exec_warehouse_placement_interior_0_int_warehouse_m_dlc_milo");

			// Warehouse Large: 1006.967, -3102.079, -39.0035
			API.RequestIpl("ex_exec_warehouse_placement_interior_2_int_warehouse_l_dlc_milo");

			// Import / Export Garage: 994.593, -3002.594, -39.647
			API.RequestIpl("imp_impexp_interior_placement");
			API.RequestIpl("imp_impexp_interior_placement_interior_0_impexp_int_01_milo_");
			API.RequestIpl("imp_impexp_interior_placement_interior_1_impexp_intwaremed_milo_");
			API.RequestIpl("imp_impexp_interior_placement_interior_2_imptexp_mod_int_01_milo_");
			API.RequestIpl("imp_impexp_interior_placement_interior_3_impexp_int_02_milo_");

			// Import / Export Garages: Interiors
			API.RequestIpl("imp_dt1_02_modgarage");
			API.RequestIpl("imp_dt1_02_cargarage_a");
			API.RequestIpl("imp_dt1_02_cargarage_b");
			API.RequestIpl("imp_dt1_02_cargarage_c");

			API.RequestIpl("imp_dt1_11_modgarage");
			API.RequestIpl("imp_dt1_11_cargarage_a");
			API.RequestIpl("imp_dt1_11_cargarage_b");
			API.RequestIpl("imp_dt1_11_cargarage_c");

			API.RequestIpl("imp_sm_13_modgarage");
			API.RequestIpl("imp_sm_13_cargarage_a");
			API.RequestIpl("imp_sm_13_cargarage_b");
			API.RequestIpl("imp_sm_13_cargarage_c");

			API.RequestIpl("imp_sm_15_modgarage");
			API.RequestIpl("imp_sm_15_cargarage_a");
			API.RequestIpl("imp_sm_15_cargarage_b");
			API.RequestIpl("imp_sm_15_cargarage_c");

			// Bunkers: Exteriors
			API.RequestIpl("gr_case0_bunkerclosed");// 848.6175, 2996.567, 45.81612
			API.RequestIpl("gr_case1_bunkerclosed");// 2126.785, 3335.04, 48.21422
			API.RequestIpl("gr_case2_bunkerclosed");// 2493.654, 3140.399, 51.28789
			API.RequestIpl("gr_case3_bunkerclosed");// 481.0465, 2995.135, 43.96672
			API.RequestIpl("gr_case4_bunkerclosed");// - 391.3216, 4363.728, 58.65862
			API.RequestIpl("gr_case5_bunkerclosed");// 1823.961, 4708.14, 42.4991
			API.RequestIpl("gr_case6_bunkerclosed");// 1570.372, 2254.549, 78.89397
			API.RequestIpl("gr_case7_bunkerclosed");// - 783.0755, 5934.686, 24.31475
			API.RequestIpl("gr_case9_bunkerclosed");// 24.43542, 2959.705, 58.35517
			API.RequestIpl("gr_case10_bunkerclosed");// - 3058.714, 3329.19, 12.5844
			API.RequestIpl("gr_case11_bunkerclosed");// - 3180.466, 1374.192, 19.9597

			// Smugglers run / Doomsday interiors

			API.RequestIpl("xm_siloentranceclosed_x17");
			API.RequestIpl("sm_smugdlc_interior_placement");
			API.RequestIpl("sm_smugdlc_interior_placement_interior_0_smugdlc_int_01_milo_");
			API.RequestIpl("xm_x17dlc_int_placement");

			API.RequestIpl("xm_x17dlc_int_placement_interior_0_x17dlc_int_base_ent_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_1_x17dlc_int_base_loop_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_2_x17dlc_int_bse_tun_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_3_x17dlc_int_base_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_4_x17dlc_int_facility_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_5_x17dlc_int_facility2_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_6_x17dlc_int_silo_01_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_7_x17dlc_int_silo_02_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_8_x17dlc_int_sub_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_9_x17dlc_int_01_milo_");

			API.RequestIpl("xm_x17dlc_int_placement_interior_10_x17dlc_int_tun_straight_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_11_x17dlc_int_tun_slope_flat_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_12_x17dlc_int_tun_flat_slope_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_13_x17dlc_int_tun_30d_r_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_14_x17dlc_int_tun_30d_l_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_15_x17dlc_int_tun_straight_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_16_x17dlc_int_tun_straight_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_17_x17dlc_int_tun_slope_flat_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_18_x17dlc_int_tun_slope_flat_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_19_x17dlc_int_tun_flat_slope_milo_");

			API.RequestIpl("xm_x17dlc_int_placement_interior_20_x17dlc_int_tun_flat_slope_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_21_x17dlc_int_tun_30d_r_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_22_x17dlc_int_tun_30d_r_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_23_x17dlc_int_tun_30d_r_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_24_x17dlc_int_tun_30d_r_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_25_x17dlc_int_tun_30d_l_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_26_x17dlc_int_tun_30d_l_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_27_x17dlc_int_tun_30d_l_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_28_x17dlc_int_tun_30d_l_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_29_x17dlc_int_tun_30d_l_milo_");

			API.RequestIpl("xm_x17dlc_int_placement_interior_30_v_apart_midspaz_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_31_v_studio_lo_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_32_v_garagem_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_33_x17dlc_int_02_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_34_x17dlc_int_lab_milo_");
			API.RequestIpl("xm_x17dlc_int_placement_interior_35_x17dlc_int_tun_entry_milo_");

			API.RequestIpl("xm_x17dlc_int_placement_strm_0");
			API.RequestIpl("xm_bunkerentrance_door");
			API.RequestIpl("xm_hatch_01_cutscene");
			API.RequestIpl("xm_hatch_02_cutscene");
			API.RequestIpl("xm_hatch_03_cutscene");
			API.RequestIpl("xm_hatch_04_cutscene");
			API.RequestIpl("xm_hatch_06_cutscene");
			API.RequestIpl("xm_hatch_07_cutscene");
			API.RequestIpl("xm_hatch_08_cutscene");
			API.RequestIpl("xm_hatch_09_cutscene");
			API.RequestIpl("xm_hatch_10_cutscene");
			API.RequestIpl("xm_hatch_closed");
			API.RequestIpl("xm_hatches_terrain");
			API.RequestIpl("xm_hatches_terrain_lod");
			API.RequestIpl("xm_mpchristmasadditions");

			// Bunkers: Interior: 892.638, -3245.866, -98.265
			//API.RequestIpl("gr_entrance_placement");
			//API.RequestIpl("gr_grdlc_interior_placement");
			//API.RequestIpl("gr_grdlc_interior_placement_interior_0_grdlc_int_01_milo_");
			//API.RequestIpl("gr_grdlc_interior_placement_interior_1_grdlc_int_02_milo_");

			//North Yankton: 3217.697, -4834.826, 111.815
			/*
			API.RequestIpl("prologue01");

			API.RequestIpl("prologue01c");

			API.RequestIpl("prologue01d");

			API.RequestIpl("prologue01e");

			API.RequestIpl("prologue01f");

			API.RequestIpl("prologue01g");

			API.RequestIpl("prologue01h");

			API.RequestIpl("prologue01i");

			API.RequestIpl("prologue01j");

			API.RequestIpl("prologue01k");

			API.RequestIpl("prologue01z");

			API.RequestIpl("prologue02");

			API.RequestIpl("prologue03");

			API.RequestIpl("prologue03b");

			API.RequestIpl("prologue04");

			API.RequestIpl("prologue04b");

			API.RequestIpl("prologue05");

			API.RequestIpl("prologue05b");

			API.RequestIpl("prologue06");

			API.RequestIpl("prologue06b");

			API.RequestIpl("prologue06_int");

			API.RequestIpl("prologuerd");

			API.RequestIpl("prologuerdb");

			API.RequestIpl("prologue_DistantLights");

			API.RequestIpl("prologue_LODLights");

			API.RequestIpl("prologue_m2_door");
			*/
		}
	}
}
