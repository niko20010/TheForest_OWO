﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MelonLoader;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Water_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Shivering_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Diving_mrse = new ManualResetEvent(false);
        public Dictionary<String, FileInfo> FeedbackMap = new Dictionary<String, FileInfo>();

        private static bHaptics.RotationOption defaultRotationOption = new bHaptics.RotationOption(0.0f, 0.0f);

        public void HeartBeatFunc()
        {
            while (true)
            {
                HeartBeat_mrse.WaitOne();
                bHaptics.SubmitRegistered("HeartBeat");
                Thread.Sleep(600);
            }
        }

        public void WaterFunc()
        {
            while (true)
            {
                Water_mrse.WaitOne();
                bHaptics.SubmitRegistered("WaterSlushing");
                Thread.Sleep(5050);
            }
        }

        public void ShiveringFunc()
        {
            while (true)
            {
                Shivering_mrse.WaitOne();
                bHaptics.SubmitRegistered("Shiver");
                Thread.Sleep(1050);
            }
        }

        public void DivingFunc()
        {
            while (true)
            {
                Diving_mrse.WaitOne();
                bHaptics.SubmitRegistered("Diving");
                Thread.Sleep(3050);
            }
        }

        public TactsuitVR()
        {
            LOG("Initializing suit");
            if (!bHaptics.WasError)
            {
                suitDisabled = false;
            }
            RegisterAllTactFiles();
            LOG("Starting HeartBeat and NeckTingle thread...");
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
            Thread WaterThread = new Thread(WaterFunc);
            WaterThread.Start();
            Thread ShiveringThread = new Thread(ShiveringFunc);
            ShiveringThread.Start();
            Thread DivingThread = new Thread(DivingFunc);
            DivingThread.Start();
        }

        public void LOG(string logStr)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            MelonLogger.Msg(logStr);
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        }



        void RegisterAllTactFiles()
        {
            string configPath = Directory.GetCurrentDirectory() + "\\Mods\\bHaptics";
            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.tact", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                // LOG("Trying to register: " + prefix + " " + fullName);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    bHaptics.RegisterFeedbackFromTactFile(prefix, tactFileStr);
                    LOG("Pattern registered: " + prefix);
                }
                catch (Exception e) { LOG(e.ToString()); }

                FeedbackMap.Add(prefix, Files[i]);
            }
            systemInitialized = true;
            //PlaybackHaptics("HeartBeat");
        }

        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f)
        {
            //LOG("Trying to play");
            if (FeedbackMap.ContainsKey(key))
            {
                //LOG("ScaleOption");
                bHaptics.ScaleOption scaleOption = new bHaptics.ScaleOption(intensity, duration);
                //LOG("Submit");
                bHaptics.SubmitRegistered(key, key, scaleOption, defaultRotationOption);
                // LOG("Playing back: " + key);
            }
            else
            {
                LOG("Feedback not registered: " + key);
            }
        }

        public void PlayBackHit(String key, float xzAngle, float yShift, float intensity = 1.0f)
        {
            bHaptics.ScaleOption scaleOption = new bHaptics.ScaleOption(intensity, 1f);
            bHaptics.RotationOption rotationOption = new bHaptics.RotationOption(xzAngle, yShift);
            bHaptics.SubmitRegistered(key, key, scaleOption, rotationOption);
        }

        public void Recoil(string weaponName, bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            var scaleOption = new bHaptics.ScaleOption(intensity, duration);
            var rotationFront = new bHaptics.RotationOption(0f, 0f);
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string keyArm = "Recoil" + postfix;
            string keyVest = "Recoil" + weaponName + "Vest" + postfix;
            string keyHands = "RecoilHands" + postfix;
            bHaptics.SubmitRegistered(keyHands, keyHands, scaleOption, rotationFront);
            bHaptics.SubmitRegistered(keyArm, keyArm, scaleOption, rotationFront);
            bHaptics.SubmitRegistered(keyVest, keyVest, scaleOption, rotationFront);
        }


        public void GunRecoil(bool isRightHand, float intensity = 1.0f )
        {
            float duration = 1.0f;
            var scaleOption = new bHaptics.ScaleOption(intensity, duration);
            var rotationFront = new bHaptics.RotationOption(0f, 0f);
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string keyArm = "Recoil" + postfix;
            string keyVest = "RecoilVest" + postfix;
            string keyHands = "RecoilHands" + postfix;
            bHaptics.SubmitRegistered(keyHands, keyHands, scaleOption, rotationFront);
            bHaptics.SubmitRegistered(keyArm, keyArm, scaleOption, rotationFront);
            bHaptics.SubmitRegistered(keyVest, keyVest, scaleOption, rotationFront);
        }

        public void SwordRecoil(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            var scaleOption = new bHaptics.ScaleOption(intensity, duration);
            var rotationFront = new bHaptics.RotationOption(0f, 0f);
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string keyArm = "RecoilBlade" + postfix;
            string keyVest = "RecoilBladeVest" + postfix;
            string keyHands = "RecoilBladeHands" + postfix;
            bHaptics.SubmitRegistered(keyHands, keyHands, scaleOption, rotationFront);
            bHaptics.SubmitRegistered(keyArm, keyArm, scaleOption, rotationFront);
            bHaptics.SubmitRegistered(keyVest, keyVest, scaleOption, rotationFront);
        }

        public void HeadShot(String key, float hitAngle)
        {
            if (bHaptics.IsDeviceConnected(bHaptics.DeviceType.Tactal))
            {
                if ((hitAngle < 45f) | (hitAngle > 315f)) { PlaybackHaptics("Headshot_F"); }
                if ((hitAngle > 45f) && (hitAngle < 135f)) { PlaybackHaptics("Headshot_L"); }
                if ((hitAngle > 135f) && (hitAngle < 225f)) { PlaybackHaptics("Headshot_B"); }
                if ((hitAngle > 225f) && (hitAngle < 315f)) { PlaybackHaptics("Headshot_R"); }
            }
            else { PlayBackHit(key, hitAngle, 0.5f); }
        }

        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }

        public void StartRain()
        {
            Water_mrse.Set();
        }

        public void StopRain()
        {
            Water_mrse.Reset();
            StopHapticFeedback("Raining");
        }

        public void StartShiver()
        {
            Shivering_mrse.Set();
        }

        public void StopShiver()
        {
            Shivering_mrse.Reset();
        }

        public void StartDiving()
        {
            Diving_mrse.Set();
        }

        public void StopDiving()
        {
            Diving_mrse.Reset();
            StopHapticFeedback("Diving");
        }

        public bool IsPlaying(String effect)
        {
            return bHaptics.IsPlaying(effect);
        }

        public void StopHapticFeedback(String effect)
        {
            bHaptics.TurnOff(effect);
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            foreach (String key in FeedbackMap.Keys)
            {
                bHaptics.TurnOff(key);
            }
        }

        public void StopThreads()
        {
            StopHeartBeat();
            StopDiving();
            StopRain();
            StopShiver();
        }


    }
}
