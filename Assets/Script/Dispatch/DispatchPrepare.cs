using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispatchPrepare : MonoBehaviour
{
 public ShowDispatchLocation showDispatchLocation;
 public ShowDispatchDragonUI showDispatchDragonUI;

 private void OnEnable()
 {
  showDispatchLocation.gameObject.SetActive(true);
  showDispatchDragonUI.gameObject.SetActive(true);
 }
 private void OnDisable()
 {
  showDispatchLocation.gameObject.SetActive(false);
  showDispatchDragonUI.gameObject.SetActive(false);
 }
}
