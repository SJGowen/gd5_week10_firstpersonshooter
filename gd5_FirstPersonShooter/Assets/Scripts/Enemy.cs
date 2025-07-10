using UnityEngine;
using UnityEngine.AI;

public class Enemy : Shootable
{
    void Start()
    {
        foreach (Rigidbody ragdollRB in GetComponentsInChildren<Rigidbody>())
        {
            ragdollRB.isKinematic = true;
        }
    }

    // Update is called once per frame
    public void TriggerRagdoll()
    {
        foreach (Rigidbody ragdollRB in GetComponentsInChildren<Rigidbody>())
        {
            ragdollRB.isKinematic = false;
        }

        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}
