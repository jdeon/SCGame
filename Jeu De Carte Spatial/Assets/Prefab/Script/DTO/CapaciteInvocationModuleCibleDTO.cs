using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapaciteInvocationModuleCibleDTO", menuName = "Mes Objets/Capacite/InvocationModuleDTO")]
public class CapaciteInvocationModuleCibleDTO : CapaciteDTO {

	public enum TypeInvocation {ajouter, ameliore , deteriore, remplace, remplaceTout};

	public CarteModuleDTO moduleAInvoquer;

	public TypeInvocation comportementSiModulSimilaire;
}
