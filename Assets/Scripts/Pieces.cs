using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces{
    public string pname;
    public int x, y;
    public Pieces(string _name, int _x, int _y){
        this.pname = _name;
        this.x = _x;
        this.y = _y;
        //Debug.Log("piesces"+pname+","+x.ToString()+y.ToString()+"创建成功!!"); 
    }
}