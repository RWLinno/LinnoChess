using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PlayerDataManager : MonoBehaviour {
    public Text manual,turn,time;
    public static int turn_num;
    public static List<string> oplist = new List<string>();
    public static string operations;
    public Transform ChessBoard;
    public static GameObject[,] pmap = new GameObject[9,9];
    public GameObject pieceobject;
    public GameObject inputpara;
    void Start()
    {
        Restart();
    }
    void Update(){
        manual.text = operations;
        DateTime nowtime = DateTime.Now.ToLocalTime();
        time.text = nowtime.ToString("HH:mm:ss");
        turn.text = "Step : " + turn_num.ToString();
    }
    static bool Check_String(string input){
        // 使用正则表达式匹配字母和数字
        return Regex.IsMatch(input, "^[a-zA-Z0-9]+$");
    }
    public void Restart(){
        Clear_All();
        LoadPlayerData("init_case");
    }
    public string get_upper(char x,char y){
        if(x=='k'&&y=='i') return "K";
        if(x=='p'&&y=='a') return "P";
        if(x=='r'&&y=='o') return "R";
        if(x=='k'&&y=='n') return "N";
        if(x=='q'&&y=='u') return "Q";
        if(x=='b'&&y=='i') return "B";
        return "";
    }
    public string get_col(int x){
        char res = (char)(x+96);
        return res.ToString();
    }
    public void Load(){
        string ps = inputpara.GetComponent<Text>().text;
        if(ps == "" || !Check_String(ps)){
            Debug.Log("empty or illegal parameter!!~");
            return;
        }else{
            string fileDestTextpath = @"Assets\Resources\Datas\" + ps + ".csv";
            if(!File.Exists(fileDestTextpath)){
                Debug.Log(fileDestTextpath+"Not Found!!!");
                return;
            }else{
                Clear_All();
                LoadPlayerData(ps);
            }
        }
    }
    public void Clear_OverLapped(GameObject obj,int sx,int sy,int tx,int ty){
        if(sx == tx && sy == ty) return;
        string origin = "";
        if(pmap[tx,ty]!=null){
            origin = pmap[tx,ty].GetComponent<PiecesDisplay>().p.pname;
            Out_the_Piece(pmap[tx,ty]);
        }
        string sname = obj.GetComponent<PiecesDisplay>().p.pname;
        string from = sx.ToString()+sy.ToString();
        string to = tx.ToString()+ty.ToString();
        if(sname[0]=='w') operations += "<color=white>";
        else operations += "<color=black>";
        operations += get_upper(sname[6],sname[7]) + get_col(tx)+ty.ToString()+","+"</color>";
        oplist.Add(sname+","+from+","+to+","+origin);
        pmap[sx,sy] = null;
        pmap[tx,ty] = obj;
        pmap[tx,ty].GetComponent<PiecesDisplay>().p.x = tx;
        pmap[tx,ty].GetComponent<PiecesDisplay>().p.y = ty;
    }
    public void lsit_to_manual(){
        foreach(var row in oplist){
            string[] data = row.Split(',');
            Debug.Log(row);
            string sname = data[0];
            if(sname[0]=='w') operations += "<color=white>";
            else operations += "<color=black>";
            char tx = data[2][0];
            char ty = data[2][1];
            operations += get_upper(sname[6],sname[7]) + get_col((int)(tx-48))+ty.ToString()+","+"</color>";
        }
    }
    public bool Check_in_ChessBoard(int x,int y){
        if(x>=1&&x<=8&&y>=1&&y<=8) return true;
        else return false;
    }
    public void LoadPlayerData(string path){
        Debug.Log("Load"+path);
        string[] dataArray = Resources.Load<TextAsset>("Datas/"+path).text.Split('\n');
        foreach (var row in dataArray)
        {
            string[] data = row.Split(',');
            if (data[0] == "#"){
                continue;
            }else if (data[0] == "pieces"){
                int x = int.Parse(data[3]);
                int y = int.Parse(data[4]);
                Pieces piece = new Pieces(data[2], x, y);
                Display(piece);
            }else if (data[0] == "turn"){
                turn_num = int.Parse(data[2]);
            }else if(data[0]=="operation"){
                oplist.Add(data[2]+","+data[3]+","+data[4]+","+data[5]);
            }
        }
        lsit_to_manual();
    }
    public void SavePlayerData(){
        string ps = inputpara.GetComponent<Text>().text;
        if(ps==""||!Check_String(ps)){
            Debug.Log("Please enter a correct parameter");
            return;
        }
        string path = @"Assets\Resources\Datas\" + ps + ".csv";
        if(!File.Exists(path)){
            File.Create(path).Dispose();
        }
        List<string> datas = new List<string>();
        datas.Add("#,id,name,x,y");
        int idx = 0;
        for(int i = 1; i <= 8;++i){
            for(int j = 1;j <= 8; ++j){
                if(pmap[i,j] != null){
                    string ns = pmap[i,j].GetComponent<PiecesDisplay>().p.pname;
                    string xs = pmap[i,j].GetComponent<PiecesDisplay>().p.x.ToString();
                    string ys = pmap[i,j].GetComponent<PiecesDisplay>().p.y.ToString();
                    //Debug.Log("Add pieces,"+idx.ToString()+","+ns+","+xs+","+ys);
                    datas.Add("pieces,"+idx.ToString()+","+ns+","+xs+","+ys);
                    ++idx;
                }
            }
        }
        datas.Add("turn,"+idx.ToString()+","+turn_num.ToString());
        ++idx;
        foreach(var v in oplist){
            datas.Add("operation,"+idx.ToString()+","+v);
            ++idx;
        }
        File.WriteAllLines(path,datas);
        Exit();
    }
    public void Display(Pieces p){ 
        int sx = 32*p.x - 16 + (int)ChessBoard.position.x - 128;
        int sy = 32*p.y - 16 + (int)ChessBoard.position.y - 128;
        GameObject newp = GameObject.Instantiate(pieceobject, ChessBoard);
        newp.GetComponent<Transform>().position = new Vector3(sx,sy,0);
        newp.AddComponent<Image>();
        newp.GetComponent<PiecesDisplay>().p = p;
        Sprite imageSprite = Resources.Load<Sprite>(p.pname);
        if(imageSprite!=null){
            newp.GetComponent<Image>().sprite = imageSprite;
        }else Debug.Log("Load pic failed!!");
        pmap[p.x,p.y] = newp;
    }
    public void Clear_All(){
        oplist.Clear();
        turn_num = 0;
        operations = "";
        Debug.Log("Clear_ChessBoad!");
        for(int i = 1; i <= 8;++i){
            for(int j = 1;j <= 8; ++j){
                Destroy(pmap[i,j]);
                pmap[i,j] = null;
            }
        }
    }
    public void Out_the_Piece(GameObject obj){
        if(obj.GetComponent<PiecesDisplay>().p != null){
            Pieces piece = obj.GetComponent<PiecesDisplay>().p;
            pmap[piece.x,piece.y] = null;
        }
        Destroy(obj);
    }
    public void Reverse(){
        Debug.Log("Reverse_ChessBoad!");
        GameObject[,] tmap = new GameObject[9,9];
        for(int i = 1; i <= 8;++i){
            for(int j = 1;j <= 8; ++j){
                tmap[i,j] = pmap[i,8-j+1];
                if(tmap[i,j] != null){
                    tmap[i,j].GetComponent<PiecesDisplay>().p.y = 9 - tmap[i,j].GetComponent<PiecesDisplay>().p.y; 
                    int newx = (int)tmap[i,j].GetComponent<Transform>().position.x;
                    int newy=  (int)tmap[i,j].GetComponent<Transform>().position.y;
                    tmap[i,j].GetComponent<Transform>().position = new Vector3(newx,258+108-newy,0);
                }
            }
        }
        for(int i = 1; i <= 8; ++i){
            for(int j = 1;j <= 8; ++j){
                pmap[i,j] = tmap[i,j];
            }
        }
    }
    public void Exit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void Debugp(){
        foreach(var row in oplist){
            Debug.Log(row);
        }
        /*
        for(int i = 1; i <= 8;++i){
            for(int j = 1;j <= 8; ++j){
                if(pmap[i,j] != null){
                    string ns = pmap[i,j].GetComponent<PiecesDisplay>().p.pname;
                    Debug.Log(i.ToString()+","+j.ToString()+","+" "+ns);
                }
            }
        }
        */
    }
}