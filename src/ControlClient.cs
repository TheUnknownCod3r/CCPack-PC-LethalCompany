﻿/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using LethalCompanyTestMod;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;


namespace ControlValley
{
    public class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51338;

        private Dictionary<string, CrowdDelegate> Delegate { get; set; }
        private IPEndPoint Endpoint { get; set; }
        private Queue<CrowdRequest> Requests { get; set; }
        private bool Running { get; set; }
        private bool Saving { get; set; }
        private bool Spawn { get; set; }

        private bool paused = false;
        public static Socket Socket { get; set; }

        public bool inGame = true;
        public static bool connect = false;
        public ControlClient()
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(CV_HOST), CV_PORT);
            Requests = new Queue<CrowdRequest>();
            Running = true;
            Saving = false;
            Spawn = true;
            Socket = null;
            connect = false;

            Delegate = new Dictionary<string, CrowdDelegate>()
            {
           
                {"heal_full", CrowdDelegates.HealFull},
                {"kill", CrowdDelegates.Kill},
                {"killcrew", CrowdDelegates.KillCrewmate},
                {"damage", CrowdDelegates.Damage},
                {"damagecrew", CrowdDelegates.DamageCrew},
                {"heal", CrowdDelegates.Heal},
                {"healcrew", CrowdDelegates.HealCrew},

                {"launch", CrowdDelegates.Launch},
                {"fast", CrowdDelegates.FastMove},
                {"slow", CrowdDelegates.SlowMove},
                {"hyper", CrowdDelegates.HyperMove},
                {"freeze", CrowdDelegates.Freeze},
                {"drunk", CrowdDelegates.Drunk},


                {"jumpultra", CrowdDelegates.UltraJump},
                {"jumphigh", CrowdDelegates.HighJump},
                {"jumplow", CrowdDelegates.LowJump},

                {"ohko", CrowdDelegates.OHKO},
                {"invul", CrowdDelegates.Invul},
                {"drain", CrowdDelegates.DrainStamins},
                {"restore", CrowdDelegates.RestoreStamins},
                {"infstam", CrowdDelegates.InfiniteStamina},
                {"nostam", CrowdDelegates.NoStamina},

                {"spawn_pede", CrowdDelegates.Spawn},
                {"spawn_spider", CrowdDelegates.Spawn},
                {"spawn_hoard", CrowdDelegates.Spawn},
                {"spawn_flower", CrowdDelegates.Spawn},
                {"spawn_crawl", CrowdDelegates.Spawn},
                {"spawn_blob", CrowdDelegates.Spawn},
                {"spawn_coil", CrowdDelegates.Spawn},
                {"spawn_puff", CrowdDelegates.Spawn},
                {"spawn_dog", CrowdDelegates.Spawn},
                {"spawn_giant", CrowdDelegates.Spawn},
                {"spawn_levi", CrowdDelegates.Spawn},
                {"spawn_hawk", CrowdDelegates.Spawn},
                {"spawn_girl", CrowdDelegates.Spawn},
                {"spawn_mimic", CrowdDelegates.Spawn},
                {"spawn_cracker", CrowdDelegates.Spawn},
                {"spawn_landmine", CrowdDelegates.Spawn},
                {"webs", CrowdDelegates.CreateWebs},
                {"killenemies", CrowdDelegates.KillEnemies},

                {"cspawn_pede", CrowdDelegates.CrewSpawn},
                {"cspawn_spider", CrowdDelegates.CrewSpawn},
                {"cspawn_hoard", CrowdDelegates.CrewSpawn},
                {"cspawn_flower", CrowdDelegates.CrewSpawn},
                {"cspawn_crawl", CrowdDelegates.CrewSpawn},
                {"cspawn_blob", CrowdDelegates.CrewSpawn},
                {"cspawn_coil", CrowdDelegates.CrewSpawn},
                {"cspawn_puff", CrowdDelegates.CrewSpawn},
                {"cspawn_dog", CrowdDelegates.CrewSpawn},
                {"cspawn_giant", CrowdDelegates.CrewSpawn},
                {"cspawn_levi", CrowdDelegates.CrewSpawn},
                {"cspawn_hawk", CrowdDelegates.CrewSpawn},
                {"cspawn_girl", CrowdDelegates.CrewSpawn},
                {"cspawn_cracker", CrowdDelegates.CrewSpawn},
                {"cspawn_mimic", CrowdDelegates.CrewSpawn},
                {"cspawn_landmine", CrowdDelegates.CrewSpawn},

                {"give_0", CrowdDelegates.GiveItem},
                {"give_1", CrowdDelegates.GiveItem},
                {"give_2", CrowdDelegates.GiveItem},
                {"give_3", CrowdDelegates.GiveItem},
                {"give_4", CrowdDelegates.GiveItem},
                {"give_5", CrowdDelegates.GiveItem},
                {"give_6", CrowdDelegates.GiveItem},
                {"give_7", CrowdDelegates.GiveItem},
                {"give_8", CrowdDelegates.GiveItem},
                {"give_9", CrowdDelegates.GiveItem},
                {"give_10", CrowdDelegates.GiveItem},
                {"give_11", CrowdDelegates.GiveItem},

                {"givem_tragedymask", CrowdDelegates.GiveMask},
                {"givem_comedymask", CrowdDelegates.GiveMask},

                {"cgive_0",  CrowdDelegates.GiveCrewItem},
                {"cgive_1",  CrowdDelegates.GiveCrewItem},
                {"cgive_2",  CrowdDelegates.GiveCrewItem},
                {"cgive_3",  CrowdDelegates.GiveCrewItem},
                {"cgive_4",  CrowdDelegates.GiveCrewItem},
                {"cgive_5",  CrowdDelegates.GiveCrewItem},
                {"cgive_6",  CrowdDelegates.GiveCrewItem},
                {"cgive_7",  CrowdDelegates.GiveCrewItem},
                {"cgive_8",  CrowdDelegates.GiveCrewItem},
                {"cgive_9",  CrowdDelegates.GiveCrewItem},
                {"cgive_10", CrowdDelegates.GiveCrewItem},
                {"cgive_11", CrowdDelegates.GiveCrewItem},

                {"cgivem_tragedymask", CrowdDelegates.GiveCrewMask},
                {"cgivem_comedymask", CrowdDelegates.GiveCrewMask},

                {"weather_-1", CrowdDelegates.Weather},
                {"weather_1", CrowdDelegates.Weather},
                {"weather_2", CrowdDelegates.Weather},
                {"weather_3", CrowdDelegates.Weather},
                {"weather_4", CrowdDelegates.Weather},
                {"weather_5", CrowdDelegates.Weather},
                {"weather_6", CrowdDelegates.Weather},
                {"lightning", CrowdDelegates.Lightning},

                {"takeitem", CrowdDelegates.TakeItem},
                {"dropitem", CrowdDelegates.DropItem},
                {"takecrewitem", CrowdDelegates.TakeCrewItem},

                {"buy_0",  CrowdDelegates.BuyItem},
                {"buy_1",  CrowdDelegates.BuyItem},
                {"buy_2",  CrowdDelegates.BuyItem},
                {"buy_3",  CrowdDelegates.BuyItem},
                {"buy_4",  CrowdDelegates.BuyItem},
                {"buy_5",  CrowdDelegates.BuyItem},
                {"buy_6",  CrowdDelegates.BuyItem},
                {"buy_7",  CrowdDelegates.BuyItem},
                {"buy_8",  CrowdDelegates.BuyItem},
                {"buy_9",  CrowdDelegates.BuyItem},
                {"buy_10", CrowdDelegates.BuyItem},
                {"buy_11", CrowdDelegates.BuyItem},

                {"charge", CrowdDelegates.ChargeItem},
                {"uncharge", CrowdDelegates.UnchargeItem},

                {"breakerson", CrowdDelegates.BreakersOn},
                {"breakersoff", CrowdDelegates.BreakersOff},

                {"toship", CrowdDelegates.TeleportToShip},
                {"crewship", CrowdDelegates.TeleportCrewToShip},
                {"body", CrowdDelegates.SpawnBody},
                {"crewbody", CrowdDelegates.SpawnCrewBody},
                {"nightvision", CrowdDelegates.NightVision},
                {"revive", CrowdDelegates.Revive},
                {"tocrew", CrowdDelegates.TeleportToCrew},
                {"crewto", CrowdDelegates.TeleportCrewTo},

                {"screech", CrowdDelegates.Screech},
                {"footstep", CrowdDelegates.Footstep},
                {"breathing", CrowdDelegates.Breathing},
                {"ghost", CrowdDelegates.Ghost},
                {"horn", CrowdDelegates.PlayHorn},
                {"blob", CrowdDelegates.BlobSound},
                {"highpitch", CrowdDelegates.HighPitch},
                {"lowpitch", CrowdDelegates.LowPitch},

                {"addhour", CrowdDelegates.AddHour},
                {"remhour", CrowdDelegates.RemoveHour},
                {"addday", CrowdDelegates.AddDay},
                {"remday", CrowdDelegates.RemoveDay},

                {"givecred_5", CrowdDelegates.AddCredits},
                {"givecred_50", CrowdDelegates.AddCredits},
                {"givecred_500", CrowdDelegates.AddCredits},
                {"givecred_-5", CrowdDelegates.AddCredits},
                {"givecred_-50", CrowdDelegates.AddCredits},
                {"givecred_-500", CrowdDelegates.AddCredits},

                {"givequota_5", CrowdDelegates.AddQuota},
                {"givequota_50", CrowdDelegates.AddQuota},
                {"givequota_500", CrowdDelegates.AddQuota},
                {"givequota_-5", CrowdDelegates.AddQuota},
                {"givequota_-50", CrowdDelegates.AddQuota},
                {"givequota_-500", CrowdDelegates.AddQuota},

                {"giveprofit_25", CrowdDelegates.AddProfit},
                {"giveprofit_50", CrowdDelegates.AddProfit},
                {"giveprofit_100", CrowdDelegates.AddProfit},
                {"giveprofit_-25", CrowdDelegates.AddProfit},
                {"giveprofit_-50", CrowdDelegates.AddProfit},
                {"giveprofit_-100", CrowdDelegates.AddProfit},
                {"addscrap", CrowdDelegates.AddScrap},

                {"shipleave", CrowdDelegates.ShipLeave},
                {"opendoors", CrowdDelegates.OpenDoors},
                {"closedoors", CrowdDelegates.CloseDoors},
            };
        }

        public static void HideEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTVISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void ShowEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_VISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void DisableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTSELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void EnableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_SELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }
        private void ClientLoop()
        {

            TestMod.mls.LogInfo("Connected to Crowd Control");
            connect = true;

            var timer = new Timer(timeUpdate, null, 0, 200);

            try
            {
                while (Running)
                {
                    CrowdRequest req = CrowdRequest.Recieve(this, Socket);
                    if (req == null || req.IsKeepAlive()) continue;

                    lock (Requests)
                        Requests.Enqueue(req);
                }
            }
            catch (Exception)
            {
                TestMod.mls.LogInfo("Disconnected from Crowd Control");
                connect = false;
                Socket.Close();
            }
        }

        public void timeUpdate(System.Object state)
        {
            inGame = true;

            if(StartOfRound.Instance == null || StartOfRound.Instance.allPlayersDead || StartOfRound.Instance.livingPlayers < 1) inGame = false;

            if (Saving || !inGame)
            {
                BuffThread.addTime(200);
                paused = true;
            } else if(paused)
            {
                paused = false;
                BuffThread.unPause();
                BuffThread.tickTime(200);
            }  else
            {
                BuffThread.tickTime(200);
            }
        }

        public bool CanSpawn() => Spawn;
        public bool IsRunning() => Running;

        public void NetworkLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {
                
                TestMod.mls.LogInfo("Attempting to connect to Crowd Control");

                try
                {
                    Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                        ClientLoop();
                    else
                        TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                    Socket.Close();
                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                }

                Thread.Sleep(10000);
            }
        }

        public void RequestLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {
                try
                {
                    while (Saving || !inGame)
                        Thread.Yield();

                    CrowdRequest req = null;
                    lock (Requests)
                    {
                        if (Requests.Count == 0)
                            continue;
                        req = Requests.Dequeue();
                    }

                    string code = req.GetReqCode();
                    try
                    {
                        CrowdResponse res;
                        if (!isReady())
                            res = new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                        else
                            res = Delegate[code](this, req);
                        if (res == null)
                        {
                            new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                        }

                        res.Send(Socket);
                    }
                    catch (KeyNotFoundException)
                    {
                        new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                    }
                }
                catch (Exception)
                {
                    TestMod.mls.LogInfo("Disconnected from Crowd Control");
                    Socket.Close();
                }
            }
        }

        public bool isReady()
        {
            try
            {
                //TestMod.mls.LogInfo($"landed: {StartOfRound.Instance.shipHasLanded}");
                //TestMod.mls.LogInfo($"planet: {RoundManager.Instance.currentLevel.PlanetName}");

                if (!StartOfRound.Instance.shipHasLanded) return false;

                if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("gordion")) return false;
                if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("company")) return false;
            }
            catch(Exception e) {
                TestMod.mls.LogError(e.ToString());
                return false;
            }

            return true;
        }

        public void Stop()
        {
            Running = false;
        }

    }
}
