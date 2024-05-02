﻿using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace TONEX;
/*
[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.Update))]
public static class FindAGameManagerUpdatePatch
{
    private static int buffer = 80;
    private static GameObject RefreshButton;
    private static GameObject InputDisplayGlyph;
    public static void Postfix(FindAGameManager __instance)
    {
        if ((RefreshButton = GameObject.Find("RefreshButton")) != null)
            RefreshButton.transform.localPosition = new Vector3(100f, 100f, 100f);
        if ((InputDisplayGlyph = GameObject.Find("InputDisplayGlyph")) != null)
            InputDisplayGlyph.transform.localPosition = new Vector3(100f, 100f, 100f);

        buffer--; if (buffer > 0) return; buffer = 80;
        __instance.RefreshList();
    }
}*/

[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.HandleList))]
public static class FindAGameManagerHandleListPatch
{
    public static void Prefix(FindAGameManager __instance, [HarmonyArgument(0)] InnerNetClient.TotalGameData totalGames, [HarmonyArgument(1)] ref List<GameListing> games)
    {
        var nameList = TranslationController.Instance.currentLanguage.languageID is SupportedLangs.SChinese or SupportedLangs.TChinese ? Main.TName_Snacks_CN : Main.TName_Snacks_EN;
        Logger.Info("0", "test");
        for (int i = 0; i < games.Count; i++)
        {
            var game = games[i];

            if (game.Language.ToString().Length > 9) continue;
            Logger.Info("1", "test");
            var color = game.Platform switch
            {
                Platforms.StandaloneItch or
                Platforms.StandaloneWin10 or
                Platforms.StandaloneEpicPC or
                Platforms.StandaloneSteamPC => "#00a4ff",

                Platforms.Xbox or
                Platforms.Switch or
                Platforms.Playstation => "#dd001b",

                Platforms.IPhone or
                Platforms.Android => "#68bc71",

                Platforms.Unknown or
                _ => "#ffffff"
            };
            Logger.Info("2", "test");
            string str = Math.Abs(game.GameId).ToString();
            int id = Math.Min(Math.Max(int.Parse(str.Substring(str.Length - 2, 2)), 1) * nameList.Count / 100, nameList.Count);
            Logger.Info("3", "test");
            game.HostName = $"" +
                $"<size=80%>" +
                $"<color={color}>" +
                $"{nameList[id]}" +
                $"</color>" +
                $"</size>"
                ;
            game.HostName += $"<size=30%> ({Math.Max(0, 100 - game.Age / 100)}%)</size>";
            Logger.Info("4", "test");
        }
        Logger.Info("5", "test");
        Logger.Info("6", "test");

    }
}
