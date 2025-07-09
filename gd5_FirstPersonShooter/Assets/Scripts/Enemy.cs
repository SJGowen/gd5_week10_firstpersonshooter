using UnityEngine;

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

        GetComponent<Collider>().enabled = false;
    }
}
