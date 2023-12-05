using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField]
    private Transform[] wayPoints;
    public Transform[] WayPoints { get => wayPoints; set => wayPoints = value; }
}
