using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
    private RectTransform rt;
    public GameObject pdm;
    public GameObject obj;
    public Transform ChessBoard;
    private int sx,sy;
    private Transform fatrans;
    void Start(){
        fatrans = transform.parent;
        rt = GetComponent<RectTransform>(); 
    }
    public void OnBeginDrag(PointerEventData eventData){
       // Debug.Log("开始拖拽"+rt.name);
       sx = (int)(rt.GetComponent<Transform>().position.x - fatrans.position.x + 128);
       sy = (int)(rt.GetComponent<Transform>().position.y - fatrans.position.y + 128);
       sx = sx / 32 + 1;
       sy = sy / 32 + 1;
    }
    public void OnDrag(PointerEventData eventData){
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.enterEventCamera, out pos);
        rt.position = pos;
    }

    public void OnEndDrag(PointerEventData eventData){
        Vector3 pos = rt.GetComponent<Transform>().position;
        int gx = (int)(pos.x - fatrans.position.x + 128) / 32 + 1;
        int gy = (int)(pos.y - fatrans.position.y + 128) / 32 + 1;
        Debug.Log("pixel:"+pos.x.ToString()+","+pos.y.ToString()+gx.ToString()+","+gy.ToString());
        if(pdm.GetComponent<PlayerDataManager>().Check_in_ChessBoard(gx,gy)){
            Debug.Log("From:"+sx.ToString()+","+sy.ToString()+",To:"+gx.ToString()+","+gy.ToString());
            pdm.GetComponent<PlayerDataManager>().Clear_OverLapped(obj,sx,sy,gx,gy);
            pos.x = (((int)pos.x / 32 )) * 32 + 20; 
            pos.y = (((int)pos.y / 32 )) * 32 + 8;
            rt.position = pos;
            PlayerDataManager.turn_num += 1;
        }else{
            pdm.GetComponent<PlayerDataManager>().Out_the_Piece(obj);
        }
    }
}