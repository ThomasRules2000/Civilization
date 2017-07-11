using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Civilizations
{
    public static int defaultCivsLength = Enum.GetNames(typeof(defaultStartingCivs)).Length;
    public static int branchingCivsLength = Enum.GetNames(typeof(branchingCivs)).Length;

    public static Dictionary<int,Civilization> civs = new Dictionary<int, Civilization>
    {
        //Starting Civs
        { (int)defaultStartingCivs.Aztecs,  new Civilization(new Color(0.921f,0.69f,0.208f), new Color(0.867f,0.118f,0.184f), "Aztecs", "Aztecs", "Aztec") },
        { (int)defaultStartingCivs.Celts,   new Civilization(new Color(0,0.5f,0), Color.white, "Celts", "Celts", "Celtic") },
        { (int)defaultStartingCivs.China,   new Civilization(Color.green,Color.white, "China","China", "Chinese") },
        { (int)defaultStartingCivs.Egypt,   new Civilization(Color.yellow, new Color(1,0,1), "Egypt", "Egypt", "Egyptian") },
        { (int)defaultStartingCivs.England, new Civilization(Color.red, Color.white, "England", "England", "English") },
        { (int)defaultStartingCivs.France,  new Civilization(new Color(0.5f,0.5f,1), Color.white, "France", "France", "French") },
        { (int)defaultStartingCivs.Germany, new Civilization(Color.white,Color.black,"Germany", "Germany", "German") },
        { (int)defaultStartingCivs.Greece,  new Civilization(Color.white,new Color(0.5f, 0.5f, 1), "Greece","Greece", "Greek") },
        { (int)defaultStartingCivs.India,   new Civilization(Color.green,Color.yellow, "India", "India", "Indian") },
        { (int)defaultStartingCivs.Korea,   new Civilization(Color.blue,Color.red, "Korea", "Korea", "Korean") },
        { (int)defaultStartingCivs.Netherlands, new Civilization(new Color(1,0.5f,0), Color.white, "Netherlands", "The Netherlands", "Dutch") },
        { (int)defaultStartingCivs.Rome,    new Civilization(new Color(1,0,1), Color.yellow, "Rome", "The Roman Empire", "Roman" ) },
        { (int)defaultStartingCivs.Russia,  new Civilization(Color.yellow,Color.black,"Russia", "Russia", "Russian") },
        { (int)defaultStartingCivs.Spain,   new Civilization(new Color(0.565f,0.302f,0.239f), new Color(0.847f,0.306f,0.125f), "Spain", "Spain", "Spanish") },
        { (int)defaultStartingCivs.USA,     new Civilization(Color.blue, Color.white, "USA", "United States of America", "American") },
        
        //Branching Civs
        { (int)branchingCivs.CSA + defaultCivsLength + 1,     new Civilization(Color.red, Color.blue, "CSA", "Confederate States of America", "Confederate") },
        { (int)branchingCivs.DPRK + defaultCivsLength + 1,    new Civilization(Color.red,Color.blue, "DPRK", "Democratic People's Republic of Korea", "North Korean") },
        { (int)branchingCivs.PRC + defaultCivsLength + 1,     new Civilization(Color.red,Color.yellow, "PRC", "People's Republic of China", "PRC") },
        { (int)branchingCivs.USSR + defaultCivsLength + 1,    new Civilization(Color.red,Color.yellow,"USSR", "Union of Soviet Socialist Republics", "Soviet") }

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

    public Color PrimaryColour
    {
        get
        {
            return primaryCol;
        }
    }

    public Color SecondaryColour
    {
        get
        {
            return secondaryCol;
        }
    }
}

public enum defaultStartingCivs
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
}

public enum branchingCivs
{
    CSA,
    DPRK,
    PRC,
    USSR
}

public enum cityStates
{

}