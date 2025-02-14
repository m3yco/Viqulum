﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Oracle.ManagedDataAccess.Client;
using System;
using TMPro;

public class ChangeScene : MonoBehaviour {
    private string result;

    //Hier werden die Szenenwechseln mit der richtigen Parameter gefüllt und gesetzt.
    public void JumpForward()
    {
        TextMeshProUGUI txt;
        String select;
        //Es wird jedesmal wenn ein wechsel stattfindet ein anderer select durchgeführt der die Informationen
        //der jeweiligen Szene ladet. 0 --> Viculum Hauptmenue, 1 --> Studiengangswahl usw.
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                //Den Text des gedrückten Buttons wird zugewiesen.
                txt = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "/Text").GetComponent<TextMeshProUGUI>();
                //Der Select der den Welchsel zurück organisiert.
                select = "select fakultaetid from fakultaet where bezeichnung = '"+ txt.text + "'";
                //Die Id aus dem Select wird gespeichert in der statischen Klasse CrossSceneInformation.
                CrossSceneInformation.faculty = GetId(select);
                // Anzeigetafel Variable befüllen!
                CrossSceneInformation.anzeige.Add(txt.text);
                //Ausgabe auf der Konsole.
                Debug.Log(CrossSceneInformation.faculty);
                //Neue Scene wird geladen.
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 1:
                txt = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "/Text").GetComponent<TextMeshProUGUI>();
                select = "select studiengangid from studiengang where bezeichnung = '" + txt.text + "'";
                CrossSceneInformation.direction = GetId(select);
                // Anzeigetafel Variable befüllen!
                CrossSceneInformation.anzeige.Add(txt.text);
                Debug.Log(CrossSceneInformation.direction);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 2:
                txt = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "/Text").GetComponent<TextMeshProUGUI>();
                String semesterid = txt.text.Substring(0, 1);
                CrossSceneInformation.semester = semesterid;
                // Anzeigetafel Variable befüllen!
                CrossSceneInformation.anzeige.Add(txt.text);
                Debug.Log(CrossSceneInformation.semester);
                if (CrossSceneInformation.semester == "5" || CrossSceneInformation.semester == "7")
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
                break;
            case 3:
                txt = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "/Text").GetComponent<TextMeshProUGUI>();
                select = "select wahlrichtungid from wahlrichtung where bezeichnung = '" + txt.text + "'";
                CrossSceneInformation.extension = GetId(select);
                // Anzeigetafel Variable befüllen!
                CrossSceneInformation.anzeige.Add(txt.text);
                Debug.Log(CrossSceneInformation.extension);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case 4:
                txt = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "/Text").GetComponent<TextMeshProUGUI>();
                select = "select modulid from modul where bezeichnung='" + txt.text + "'";
                CrossSceneInformation.modul = GetId(select);
                // Anzeigetafel Variable befüllen!
                CrossSceneInformation.anzeige.Add(txt.text);
                Debug.Log(CrossSceneInformation.modul);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1,LoadSceneMode.Single);
                break;
            default:
                break;
        }
    }

    public void JumpBackward()
    {
        //Diese Methode organisiert den zurück Sprung in eine vorherige Szene.
        //Bei Szene 5 & 7 muss man 2 mal Springen um zur Wahl der Vertiefung CPS, AD, ITM zu kommen.
        if (SceneManager.GetActiveScene().buildIndex == 4 && CrossSceneInformation.semester != "5" && CrossSceneInformation.semester != "7")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
            CrossSceneInformation.anzeige.RemoveAt(CrossSceneInformation.anzeige.Count - 1);
        }
        //else if (SceneManager.GetActiveScene().buildIndex == 4 && CrossSceneInformation.semester == "5" && CrossSceneInformation.semester == "7")
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
        //    CrossSceneInformation.anzeige.RemoveAt(CrossSceneInformation.anzeige.Count - 1);
        //}
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            CrossSceneInformation.anzeige.RemoveAt(CrossSceneInformation.anzeige.Count - 1);
        }
    }

    void Update()
    {
        //Die Update Methode ist wichtig um den Mouse Courser wieder zu befreien,
        //weil er in der Raumszene als Kamera benutzt wird. 
        if(SceneManager.GetActiveScene().buildIndex == 5)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            // Hier Kommando "B"(Back) auf der Tastatur für das zurück springen gesetzt. 
            if (Input.GetKeyDown(KeyCode.B))
            {
                CrossSceneInformation.anzeige.RemoveAt(CrossSceneInformation.anzeige.Count - 1);
                Cursor.visible = true;
                SceneManager.LoadScene("Modul", LoadSceneMode.Single); 
            }

            // Hier Kommando "J"(Jump) auf der Tastatur um in die Aufbau Veranstaltung zu springen.
            if (Input.GetKeyDown(KeyCode.J))
            {
                // Schreibe statische Klasse mit neuen Informationen um
                setJumpInfo(CrossSceneInformation.jump);
                extensionExsists(CrossSceneInformation.jump);
                // --> Wenn Sprung auswertung ob dann Wahlrichtung vorhanden ist wenn ja einfügen!

                SceneManager.LoadScene("Room", LoadSceneMode.Single);
            }
        }
    }

    public string GetId(String select)
    {
        //Diese Methode wandelt den Text des gedrückten Buttons um zur
        //einer Id die gespeichert wird für die nächste Szene
        String connectionString = "Data Source=(DESCRIPTION=" +
           "(ADDRESS=(PROTOCOL=TCP)(HOST=orahost)(PORT=1521))" +
           "(CONNECT_DATA=(SERVICE_NAME=infdb.inf.hs-albsig.de)));" +
           "User Id = projektstudium; Password = projektstudium; ";
  
        OracleConnection con = new OracleConnection();

        try
        {
            con.ConnectionString = connectionString;
            con.Open();

            OracleCommand cmd = new OracleCommand(select, con);
            cmd.CommandType = System.Data.CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                long id = dr.GetInt32(0);
                result += id.ToString();
            }
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
        return result;
    }

    public void setJumpInfo(String modulName)
    {

        String connectionString = "Data Source=(DESCRIPTION=" +
           "(ADDRESS=(PROTOCOL=TCP)(HOST=orahost)(PORT=1521))" +
           "(CONNECT_DATA=(SERVICE_NAME=infdb.inf.hs-albsig.de)));" +
           "User Id = projektstudium; Password = projektstudium; ";

        OracleConnection con = new OracleConnection();
        string select = "select s.semesterid, m.modulid from semesterveranstaltung s, modul m " +
            "where s.modulid = m.modulid " +
            "and m.modulid = (select modulid from modul where bezeichnung = '"+ modulName + "')";

        long semesterid = 0;
        long modulid = 0;

        try
        {
            con.ConnectionString = connectionString;
            con.Open();

            OracleCommand cmd = new OracleCommand(select, con);
            cmd.CommandType = System.Data.CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                semesterid = dr.GetInt32(0);
                modulid = dr.GetInt32(1);
            }
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

        // Alle Variablen befüllen nicht nur Semester und modul!
        CrossSceneInformation.semester = semesterid.ToString();
        CrossSceneInformation.modul = modulid.ToString();
    }

    public void extensionExsists(String modulName)
    {
        //Abfrage ob Modul beim Sprung eine Vertiefung hat!
    }
}
