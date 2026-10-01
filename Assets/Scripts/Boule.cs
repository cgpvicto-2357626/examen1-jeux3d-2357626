using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    [SerializeField, Tooltip("Le maximum de charges à utliser")]
    private int maxCharges = 3;

    [SerializeField, Tooltip("La force d'accelélartion donnée")]
    private float forceAcceleration = 4f;

    [SerializeField, Tooltip("La durée d'accéleration")]
    private float durreAcceleration = 1.15f;

    public int Charges
    {
        get{return charges;}
    }

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    private int charges;
    private bool EnAcceleration;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        PlayerInput controles = ControleurJeu.Instance.Controles;
        controles.actions.FindAction("Diriger").performed += CommencerDirection;
        controles.actions.FindAction("Diriger").canceled += ArreterDirection;
        controles.actions.FindAction("Commencer").performed += CommencerJeu;
        controles.actions.FindAction("Accelerer").performed += UtiliserCharge;
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
        controles.actions.FindAction("Accelerer").performed -= UtiliserCharge;
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }
    }

    private void FixedUpdate()
    {
        Diriger();
    }
    /// <summary>
    /// utliser la charge quand la balle traverse l'objet acceleration
    /// </summary>
    public void AjouterCharge()
    {
        if(charges < maxCharges)
        {
            charges++;
        }
    }

    /// <summary>
    /// qunad on clique sur W on appelle cette méthode
    /// </summary>
    /// <param name="context"></param>
    private void UtiliserCharge(InputAction.CallbackContext context)
    {
        if(charges > 0 && !EnAcceleration)
        {
            StartCoroutine(Accelerer());
        }
    }

    /// <summary>
    /// Coroutine pour l'Accéleration
    /// </summary>
    /// <returns></returns>
    private IEnumerator Accelerer()
    {
        EnAcceleration = true;
        charges--;

        float duree = 0f;
        while(duree < durreAcceleration)
        {
            rigidbody.AddForce(Vector3.forward * forceAcceleration, ForceMode.Acceleration);
            duree += Time.deltaTime;
            yield return null;
        }
        EnAcceleration = false;
    }


    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * Vector3.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void CommencerJeu(InputAction.CallbackContext context)
    {
        rigidbody.useGravity = true;
        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed -= CommencerJeu;
    }

    private void Diriger()
    {
        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }
}
