using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPlatform : MonoBehaviour
{
    [HideInInspector]
    public bool isWorking;          //  Control if the platform is busy

    Material m_PlatforMaterial;     //  Material fo the object

    private void Start()
    {
        //  Get the material component and change the color
        m_PlatforMaterial = GetComponent<Renderer>().material;
        ChangeMaterial(Color.green);
    }

    public void ChangeMaterial(Color color)
    {
        //  Change the color of the material
        m_PlatforMaterial.color = color;
    }

    public IEnumerator DeactivePlatform(float activeTime, float deactiveTime)
    {
        //  Start the deactivation of the platform and then the activation
        isWorking = true;
        ChangeMaterial(Color.red);
        yield return new WaitForSeconds(activeTime);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(deactiveTime);
        gameObject.SetActive(true);
        ChangeMaterial(Color.green);
        isWorking = false;
    }
}
