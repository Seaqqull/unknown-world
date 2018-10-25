using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour {
    [SerializeField]
    Transform m_transform;

    Animator anim;
    NavMeshAgent m_navMeshAgent;

	// Use this for initialization
	void Start () {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        m_navMeshAgent.SetDestination(m_transform.position);
    }

    
}