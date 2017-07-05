using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Civilizations
{
    public static Dictionary<int,Civilization> civs = new Dictionary<int, Civilization>
    {
        //Starting Civs
        { (int)defaultCivs.Aztecs,  new Civilization() },
        { (int)defaultCivs.Celts,   new Civilization(new Color(0,0.5f,0), Color.white, "Celts", "Celts", "Celtic") },
        { (int)defaultCivs.China,   new Civilization(Color.green,Color.white, "China","China", "Chinese") },
        { (int)defaultCivs.Egypt,   new Civilization(Color.yellow, new Color(1,0,1), "Egypt", "Egypt", "Egyptian") },
        { (int)defaultCivs.England, new Civilization(Color.red, Color.white, "England", "England", "English") },
        { (int)defaultCivs.France,  new Civilization(new Color(0.5f,0.5f,1), Color.white, "France", "France", "French") },
        { (int)defaultCivs.Germany, new Civilization(Color.white,Color.black,"Germany", "Germany", "German") },
        { (int)defaultCivs.Greece,  new Civilization(Color.white,new Color(0.5f, 0.5f, 1), "Greece","Greece", "Greek") },
        { (int)defaultCivs.India,   new Civilization(Color.green,Color.yellow,,"India", "India", "Indian") },
        { (int)defaultCivs.Korea,   new Civilization(Color.blue,Color.red, "Korea", "Korea", "Korean") },
        { (int)defaultCivs.Netherlands, new Civilization(new Color(1,0.5f,0), Color.white, "Netherlands", "The Netherlands", "Dutch") },
        { (int)defaultCivs.Rome,    new Civilization(new Color(1,0,1), Color.yellow, "Rome", "The Roman Empire", "Roman" ) },
        { (int)defaultCivs.Russia,  new Civilization(Color.yellow,Color.black,"Russia", "Russia", "Russian") },
        { (int)defaultCivs.Spain,   new Civilization() },
        { (int)defaultCivs.USA,     new Civilization(Color.blue, Color.white, "USA", "United States of America", "American") },
        
        //Branching Civs
        { (int)defaultCivs.CSA,     new Civilization(Color.red, Color.blue, "CSA", "Confederate States of America", "Confederate") },
        { (int)defaultCivs.DPRK,    new Civilization(Color.red,Color.blue, "DPRK", "Democratic People's Republic of Korea", "North Korean") },
        { (int)defaultCivs.PRC,     new Civilization(Color.red,Color.yellow, "PRC", "People's Republic of China", "PRC") },
        { (int)defaultCivs.USSR,    new Civilization(Color.red,Color.yellow,"USSR", "Union of Soviet Socialist Republics", "Soviet") }

        //City States
        
    };
}

public class Civilization
{
    Color primaryCol;
    Color secondaryCol;
    //Unit uniqueUnit;
    //Building uniqueBuilding

    string civShortName;
    string civLongName;
    string civNationality;

    public Civilization(Color _primary, Color _secondary, string _shortName, string _longName, string _nationality)
    {
        primaryCol = _primary;
        secondaryCol = _secondary;
        civShortName = _shortName;
        civLongName = _longName;
        civNationality = _nationality;
    }
}

public enum defaultCivs
{
    //Starting Civs
    Aztecs,
    Celts,
    China,
    Egypt,
    England,
    France,
    Germany,
    Greece,
    India,
    Korea,
    Netherlands,
    Rome,
    Russia,
    Spain,
    USA,

    //Branching Civs
    CSA,
    DPRK,
    PRC,
    USSR
}