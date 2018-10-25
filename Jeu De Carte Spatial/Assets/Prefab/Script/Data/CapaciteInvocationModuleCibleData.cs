using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapaciteInvocationModuleCibleData", menuName = "Mes Objets/Capacite/InvocationModuleData")]
public class CapaciteInvocationModuleCibleData : CapaciteData {

	public CarteModuleData moduleAInvoquer;

	public ConstanteEnum.TypeInvocation comportementSiModulSimilaire;
}
