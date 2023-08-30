using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Create : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
    private RectTransform rt;
    private int sx,sy,color;
    public GameObject pdm;
    public Transform ChessBoard;
    void Start(){
        rt = GetComponent<RectTransform>();
        color = (rt.GetComponent<Transform>().position.y>=-20)?1:0;
    }
    public void OnBeginDrag(PointerEventData eventData){
        sx = (int)rt.GetComponent<Transform>().position.x;
        sy = (int)rt.GetComponent<Transform>().position.y;
    }
    public void OnDrag(PointerEventData eventData){
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.enterEventCamera, out pos);
        rt.position = pos;
    }
    public void OnEndDrag(PointerEventData eventData){
        // 生成新棋子
        Vector3 pos = rt.GetComponent<Transform>().position;
        int gx = (int)(pos.x - ChessBoard.GetComponent<Transform>().position.x + 128) / 32 + 1;
        int gy = (int)(pos.y - ChessBoard.GetComponent<Transform>().position.y + 128) / 32 + 1;
        // 小图标返回原来的地方
        pos.x = sx;
        pos.y = sy;
        rt.position = pos;
        Debug.Log("Create:"+gx.ToString()+","+gy.ToString());
        if(pdm.GetComponent<PlayerDataManager>().Check_in_ChessBoard(gx,gy)){
            string new_name = rt.name;
            Pieces piece = new Pieces(new_name, gx, gy);
            pdm.GetComponent<PlayerDataManager>().Display(piece);
        }
    }
}