﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oracle.ManagedDataAccess.Client;
using TMPro;
using System;


public class RoomDuration : MonoBehaviour
{
    // Zeigt die Minuten einer Veranstaltung an, aus der Datenbank.
    private TextMeshProUGUI txt;
    private string result;

    String connectionString = "Data Source=(DESCRIPTION=" +
           "(ADDRESS=(PROTOCOL=TCP)(HOST=orahost)(PORT=1521))" +
           "(CONNECT_DATA=(SERVICE_NAME=infdb.inf.hs-albsig.de)));" +
           "User Id = projektstudium; Password = projektstudium; ";


    String select = "select dauer from modul where modulid = " + CrossSceneInformation.modul + "";

    OracleConnection con = new OracleConnection();
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
        try
        {
            con.ConnectionString = connectionString;

            con.Open();

            OracleCommand cmd = new OracleCommand(select, con);
            cmd.CommandType = System.Data.CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                long Bezeichnung = dr.GetInt32(0);
                result += Bezeichnung.ToString();
            }
            txt.text = result;
            Debug.Log(result);
            dr.Dispose();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log(ex.StackTrace);
        }
        finally
        {
            con.Close();
            con.Dispose();
            con = null;
        }
    }
}
