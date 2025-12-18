using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayerController : MonoBehaviour
{
 void Awake()
 {

  if (FindObjectsOfType(typeof(BGMPlayerController)).Length > 1)
  {
   Destroy(gameObject);
   return;
  }
  DontDestroyOnLoad(gameObject);
 }
 
}
