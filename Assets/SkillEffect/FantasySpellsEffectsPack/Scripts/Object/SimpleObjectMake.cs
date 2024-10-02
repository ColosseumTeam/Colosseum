using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectMake : _ObjectMakeBase {

    [SerializeField] private Vector3 _rangeTwoSkill;
    public Vector3 m_randomRotationValue;

    void Start(){
        for(int i = 0; i < m_makeObjs.Length; i++)
        {
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            GameObject m_obj = Instantiate(m_makeObjs[i], transform.position, rotation);
            m_obj.transform.parent = this.transform;
            //m_obj.transform.rotation *= Quaternion.Euler(GetRandomVector(m_randomRotationValue));

            if (m_movePos){ 
                if(m_obj.GetComponent<MoveToObject>()){ 
                    MoveToObject m_script = m_obj.GetComponent<MoveToObject>();
                    m_script.m_movePos = m_movePos;
                }
            }
        }
    }

    private void Update()
    {
        if (_rangeTwoSkill == null)
        {
            Destroy(gameObject, 3f);
        }
        else
        {
            transform.position = _rangeTwoSkill;
        }
    }


    public void FireFieldProduce(Vector3 transform)
    {
        _rangeTwoSkill = transform;
    }
}
