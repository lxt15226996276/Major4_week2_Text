using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Exam.Exam09
{
    /// <summary>
    /// AI赛车：用NavMesh沿赛道自动行驶到终点
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
   
    public class AICarController : MonoBehaviour
    {
        [Header("寻路")]
        [SerializeField] private Transform endTarget;
     
        private NavMeshAgent agent;
       
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            endTarget=GameObject.Find("End")?.transform;

        }
        private void Start()
        {
            if(endTarget==null)return;
           agent.SetDestination(endTarget.position);

        }
    

    }
}

