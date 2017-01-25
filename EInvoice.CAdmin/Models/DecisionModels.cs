using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class DecisionModels
    {
        public int id { get; set; }
        public int ComID { get; set; }
        public string ComName { get; set; }
        public string ParentCompany { get; set; }
        public string ComAddress { get; set; }
        public string TaxCode { get; set; }
        public string DecisionNo { get;set; }
        public string Director { get; set; }
        public string Requester { get; set; }
        public string SystemName { get; set; }
        public string SoftApplication { get; set; }
        public string TechDepartment { get; set; }
        public string DecDatasource { get; set; }
        public string Workflow { get; set; }
        public string Responsibility {get;set; }
        public string City { get; set; }
        public string EffectiveDate { get; set; }
        public string EffectDate { get; set; }
        public string Destination { get; set; }
        public SelectList RegTempList { get; set; }

        public EInvoice.Core.Domain.Decision UpdateDecision(EInvoice.Core.Domain.Decision mDecision)
        {
            mDecision.id = id;
            mDecision.ComID = ComID;
            mDecision.ComName = ComName;
            mDecision.ParentCompany = ParentCompany;
            mDecision.ComAddress = ComAddress;
            mDecision.TaxCode = TaxCode;
            mDecision.DecisionNo = DecisionNo;
            mDecision.Director= Director;
            mDecision.Requester= Requester; 
            mDecision.SystemName= SystemName; 
            mDecision.SoftApplication=SoftApplication;
            mDecision.TechDepartment = TechDepartment;
            mDecision.EffectiveDate = EffectiveDate;
            mDecision.EffectDate = EffectDate;
            mDecision.Workflow = Workflow;
            mDecision.Responsibility=Responsibility;
            mDecision.City=City;
            mDecision.Destination = Destination;
            return mDecision;
        }
    }
}