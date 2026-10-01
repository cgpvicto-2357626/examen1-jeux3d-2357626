using UnityEngine;

/// <summary>
/// Script pour gerer l'acceleration du boule
/// </summary>
public class Acceleration : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boule"))
        {
            Boule boule = other.GetComponent<Boule>();
            if(boule != null)
            {
                boule.AjouterCharge();
            }
            Destroy(gameObject);
        }
    }
}
