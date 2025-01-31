﻿using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using Il2CppSystem.Linq;
using UnityEngine;

namespace TONEX;

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
}//*/

/*

[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.HandleList))]
public static class FindAGameManagerHandleListPatch
{
   public static void Prefix(FindAGameManager __instance, [HarmonyArgument(0)] InnerNetClient.TotalGameData totalGames, [HarmonyArgument(1)] ref List<GameListing> availableGames)
   {
       var nameList = TranslationController.Instance.currentLanguage.languageID is SupportedLangs.SChinese or SupportedLangs.TChinese ? Main.TName_Snacks_CN : Main.TName_Snacks_EN;
       Logger.Info("0", "test");
       for (int i = 0; i < availableGames.Count; i++)
       {
           var game = availableGames[i];

           if (game.Language.ToString().Length > 9) continue;
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
           string str = Math.Abs(game.GameId).ToString();
           int id = Math.Min(Math.Max(int.Parse(str.Substring(str.Length - 2, 2)), 1) * nameList.Count / 100, nameList.Count);
           game.HostName = $"" +
               $"<size=80%>" +
               $"<color={color}>" +
               $"{nameList[id]}" +
               $"</color>" +
               $"</size>"
               ;
           game.HostName += $"<size=30%> ({Math.Max(0, 100 - game.Age / 100)}%)</size>";
       }
   }



}
*/
[HarmonyPatch(typeof(MatchMakerGameButton), nameof(MatchMakerGameButton.SetGame))]
public static class MatchMakerGameButtonSetGamePatch
{
    public static void Prefix(MatchMakerGameButton __instance, [HarmonyArgument(0)]  GameListing game)
    {
        var nameList = TranslationController.Instance.currentLanguage.languageID is SupportedLangs.SChinese or SupportedLangs.TChinese ? Main.TName_Snacks_CN : Main.TName_Snacks_EN;
        Logger.Info("0", "test");
        

            if (game.Language.ToString().Length > 9) goto End;
            var color = game.Platform switch
            {
                Platforms.StandaloneItch => "#737373",
                Platforms.StandaloneWin10 => "#FFF88D",
                Platforms.StandaloneEpicPC => "#905CDA",
                Platforms.StandaloneSteamPC => "#3A78A8",

                Platforms.Xbox => "#07ff00",
                Platforms.Switch => "",
                Platforms.Playstation => "#001090",

                Platforms.StandaloneMac => "#e3e3e3",
                Platforms.IPhone => "#e3e3e3",
                Platforms.Android => "#47E540",

                Platforms.Unknown or
                _ => "#ffffff"
            };
        var platforms = game.Platform switch
        {
            Platforms.StandaloneItch => "Itch",
            Platforms.StandaloneWin10 => "Win10",
            Platforms.StandaloneEpicPC => "Epic",
            Platforms.StandaloneSteamPC => "Steam",
            
            Platforms.Xbox => "Xbox",
            Platforms.Switch => "",
            Platforms.Playstation => "PlayStation",

            Platforms.StandaloneMac => "Mac.",
            Platforms.IPhone => Translator.GetString("IPhone"),
            Platforms.Android => Translator.GetString("Android"),

            Platforms.Unknown or
            _ => "#ffffff"
        };
        string str = Math.Abs(game.GameId).ToString();
            int id = Math.Min(Math.Max(int.Parse(str.Substring(str.Length - 2, 2)), 1) * nameList.Count / 100, nameList.Count);
        if (game.Platform == Platforms.Switch)
        {
            int halfLength = nameList[id].Length / 2;

            string firstHalf = nameList[id].Substring(0, halfLength);
            string secondHalf = nameList[id].Substring(halfLength);
            game.HostName = $"" +
                $"<size=80%>" +

                $"<color=#00B2FF>" +
                $"{firstHalf}" +
                $"</color>" +

                $"<color=#ff0000>" +
                $"{secondHalf}" +
                $"</color>" +
                $"</size>" +

                $"<size=40%>" +

                "<color=#ffff00>----</color>" +
                $"<color=#00B2FF>" +
                $"Nintendo" +
                $"</color>" +

                $"<color=#ff0000>" +
                $"Switch" +
                $"</color>" +

                $"</size>"
                ;
        }
        else
            game.HostName = $"" +
                $"<size=80%>" +
                $"<color={color}>" +
                $"{nameList[id]}" +
                $"</color>" +
                $"</size>"+
                $"<size=40%>" +
                "<color=#ffff00>----</color>" +
                $"<color={color}>" +
                $"{platforms}" +
                $"</color>" +
                $"</size>"
                ;
            game.HostName += $"<size=30%> ({Math.Max(0, 100 - game.Age / 100)}%)</size>";
        End:
        Logger.Info("1", "test");
    }



}
