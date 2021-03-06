﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Microsoft.VSDesigner generó automáticamente este código fuente, versión=4.0.30319.42000.
// 
#pragma warning disable 1591

namespace CheckArgentina.InternationalService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="HotelsPortBinding", Namespace="http://ws.mph.nemogroup.net/")]
    public partial class Hotels : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback HotelAvailOperationCompleted;
        
        private System.Threading.SendOrPostCallback HotelDescriptiveInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback HotelResOperationCompleted;
        
        private System.Threading.SendOrPostCallback HotelBookingRuleOperationCompleted;
        
        private System.Threading.SendOrPostCallback HotelResModifyOperationCompleted;
        
        private System.Threading.SendOrPostCallback QueryBookingOperationCompleted;
        
        private System.Threading.SendOrPostCallback RateBreakDownOperationCompleted;
        
        private System.Threading.SendOrPostCallback ReadOperationCompleted;
        
        private System.Threading.SendOrPostCallback VoucherOperationCompleted;
        
        private System.Threading.SendOrPostCallback DestinationListOperationCompleted;
        
        private System.Threading.SendOrPostCallback LocationListOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Hotels() {
            this.Url = global::CheckArgentina.Properties.Settings.Default.CheckArgentina_InternationalService_Hotels;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event HotelAvailCompletedEventHandler HotelAvailCompleted;
        
        /// <remarks/>
        public event HotelDescriptiveInfoCompletedEventHandler HotelDescriptiveInfoCompleted;
        
        /// <remarks/>
        public event HotelResCompletedEventHandler HotelResCompleted;
        
        /// <remarks/>
        public event HotelBookingRuleCompletedEventHandler HotelBookingRuleCompleted;
        
        /// <remarks/>
        public event HotelResModifyCompletedEventHandler HotelResModifyCompleted;
        
        /// <remarks/>
        public event QueryBookingCompletedEventHandler QueryBookingCompleted;
        
        /// <remarks/>
        public event RateBreakDownCompletedEventHandler RateBreakDownCompleted;
        
        /// <remarks/>
        public event ReadCompletedEventHandler ReadCompleted;
        
        /// <remarks/>
        public event VoucherCompletedEventHandler VoucherCompleted;
        
        /// <remarks/>
        public event DestinationListCompletedEventHandler DestinationListCompleted;
        
        /// <remarks/>
        public event LocationListCompletedEventHandler LocationListCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("HotelAvail", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("HotelAvailRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string HotelAvail([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string HotelAvailRQ) {
            object[] results = this.Invoke("HotelAvail", new object[] {
                        HotelAvailRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HotelAvailAsync(string HotelAvailRQ) {
            this.HotelAvailAsync(HotelAvailRQ, null);
        }
        
        /// <remarks/>
        public void HotelAvailAsync(string HotelAvailRQ, object userState) {
            if ((this.HotelAvailOperationCompleted == null)) {
                this.HotelAvailOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHotelAvailOperationCompleted);
            }
            this.InvokeAsync("HotelAvail", new object[] {
                        HotelAvailRQ}, this.HotelAvailOperationCompleted, userState);
        }
        
        private void OnHotelAvailOperationCompleted(object arg) {
            if ((this.HotelAvailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HotelAvailCompleted(this, new HotelAvailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("HotelDescriptiveInfo", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("HotelDescriptiveInfoRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string HotelDescriptiveInfo([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string HotelDescriptiveInfoRQ) {
            object[] results = this.Invoke("HotelDescriptiveInfo", new object[] {
                        HotelDescriptiveInfoRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HotelDescriptiveInfoAsync(string HotelDescriptiveInfoRQ) {
            this.HotelDescriptiveInfoAsync(HotelDescriptiveInfoRQ, null);
        }
        
        /// <remarks/>
        public void HotelDescriptiveInfoAsync(string HotelDescriptiveInfoRQ, object userState) {
            if ((this.HotelDescriptiveInfoOperationCompleted == null)) {
                this.HotelDescriptiveInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHotelDescriptiveInfoOperationCompleted);
            }
            this.InvokeAsync("HotelDescriptiveInfo", new object[] {
                        HotelDescriptiveInfoRQ}, this.HotelDescriptiveInfoOperationCompleted, userState);
        }
        
        private void OnHotelDescriptiveInfoOperationCompleted(object arg) {
            if ((this.HotelDescriptiveInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HotelDescriptiveInfoCompleted(this, new HotelDescriptiveInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("HotelRes", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("HotelResRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string HotelRes([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string HotelResRQ) {
            object[] results = this.Invoke("HotelRes", new object[] {
                        HotelResRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HotelResAsync(string HotelResRQ) {
            this.HotelResAsync(HotelResRQ, null);
        }
        
        /// <remarks/>
        public void HotelResAsync(string HotelResRQ, object userState) {
            if ((this.HotelResOperationCompleted == null)) {
                this.HotelResOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHotelResOperationCompleted);
            }
            this.InvokeAsync("HotelRes", new object[] {
                        HotelResRQ}, this.HotelResOperationCompleted, userState);
        }
        
        private void OnHotelResOperationCompleted(object arg) {
            if ((this.HotelResCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HotelResCompleted(this, new HotelResCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("HotelBookingRule", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("HotelBookingRuleRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string HotelBookingRule([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string HotelBookingRuleRQ) {
            object[] results = this.Invoke("HotelBookingRule", new object[] {
                        HotelBookingRuleRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HotelBookingRuleAsync(string HotelBookingRuleRQ) {
            this.HotelBookingRuleAsync(HotelBookingRuleRQ, null);
        }
        
        /// <remarks/>
        public void HotelBookingRuleAsync(string HotelBookingRuleRQ, object userState) {
            if ((this.HotelBookingRuleOperationCompleted == null)) {
                this.HotelBookingRuleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHotelBookingRuleOperationCompleted);
            }
            this.InvokeAsync("HotelBookingRule", new object[] {
                        HotelBookingRuleRQ}, this.HotelBookingRuleOperationCompleted, userState);
        }
        
        private void OnHotelBookingRuleOperationCompleted(object arg) {
            if ((this.HotelBookingRuleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HotelBookingRuleCompleted(this, new HotelBookingRuleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("HotelResModify", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("HotelResModifyRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string HotelResModify([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string HotelResModifyRQ) {
            object[] results = this.Invoke("HotelResModify", new object[] {
                        HotelResModifyRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HotelResModifyAsync(string HotelResModifyRQ) {
            this.HotelResModifyAsync(HotelResModifyRQ, null);
        }
        
        /// <remarks/>
        public void HotelResModifyAsync(string HotelResModifyRQ, object userState) {
            if ((this.HotelResModifyOperationCompleted == null)) {
                this.HotelResModifyOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHotelResModifyOperationCompleted);
            }
            this.InvokeAsync("HotelResModify", new object[] {
                        HotelResModifyRQ}, this.HotelResModifyOperationCompleted, userState);
        }
        
        private void OnHotelResModifyOperationCompleted(object arg) {
            if ((this.HotelResModifyCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HotelResModifyCompleted(this, new HotelResModifyCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("QueryBooking", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("QueryBookingRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string QueryBooking([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string QueryBookingRQ) {
            object[] results = this.Invoke("QueryBooking", new object[] {
                        QueryBookingRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void QueryBookingAsync(string QueryBookingRQ) {
            this.QueryBookingAsync(QueryBookingRQ, null);
        }
        
        /// <remarks/>
        public void QueryBookingAsync(string QueryBookingRQ, object userState) {
            if ((this.QueryBookingOperationCompleted == null)) {
                this.QueryBookingOperationCompleted = new System.Threading.SendOrPostCallback(this.OnQueryBookingOperationCompleted);
            }
            this.InvokeAsync("QueryBooking", new object[] {
                        QueryBookingRQ}, this.QueryBookingOperationCompleted, userState);
        }
        
        private void OnQueryBookingOperationCompleted(object arg) {
            if ((this.QueryBookingCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.QueryBookingCompleted(this, new QueryBookingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("RateBreakDown", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("RateBreakDownRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string RateBreakDown([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string RateBreakDownRQ) {
            object[] results = this.Invoke("RateBreakDown", new object[] {
                        RateBreakDownRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void RateBreakDownAsync(string RateBreakDownRQ) {
            this.RateBreakDownAsync(RateBreakDownRQ, null);
        }
        
        /// <remarks/>
        public void RateBreakDownAsync(string RateBreakDownRQ, object userState) {
            if ((this.RateBreakDownOperationCompleted == null)) {
                this.RateBreakDownOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRateBreakDownOperationCompleted);
            }
            this.InvokeAsync("RateBreakDown", new object[] {
                        RateBreakDownRQ}, this.RateBreakDownOperationCompleted, userState);
        }
        
        private void OnRateBreakDownOperationCompleted(object arg) {
            if ((this.RateBreakDownCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RateBreakDownCompleted(this, new RateBreakDownCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("Read", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ReadRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string Read([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string ReadRQ) {
            object[] results = this.Invoke("Read", new object[] {
                        ReadRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ReadAsync(string ReadRQ) {
            this.ReadAsync(ReadRQ, null);
        }
        
        /// <remarks/>
        public void ReadAsync(string ReadRQ, object userState) {
            if ((this.ReadOperationCompleted == null)) {
                this.ReadOperationCompleted = new System.Threading.SendOrPostCallback(this.OnReadOperationCompleted);
            }
            this.InvokeAsync("Read", new object[] {
                        ReadRQ}, this.ReadOperationCompleted, userState);
        }
        
        private void OnReadOperationCompleted(object arg) {
            if ((this.ReadCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ReadCompleted(this, new ReadCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("Voucher", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("VoucherRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string Voucher([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string VoucherRQ) {
            object[] results = this.Invoke("Voucher", new object[] {
                        VoucherRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void VoucherAsync(string VoucherRQ) {
            this.VoucherAsync(VoucherRQ, null);
        }
        
        /// <remarks/>
        public void VoucherAsync(string VoucherRQ, object userState) {
            if ((this.VoucherOperationCompleted == null)) {
                this.VoucherOperationCompleted = new System.Threading.SendOrPostCallback(this.OnVoucherOperationCompleted);
            }
            this.InvokeAsync("Voucher", new object[] {
                        VoucherRQ}, this.VoucherOperationCompleted, userState);
        }
        
        private void OnVoucherOperationCompleted(object arg) {
            if ((this.VoucherCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.VoucherCompleted(this, new VoucherCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("DestinationList", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DestinationListRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string DestinationList([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string DestinationListRQ) {
            object[] results = this.Invoke("DestinationList", new object[] {
                        DestinationListRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void DestinationListAsync(string DestinationListRQ) {
            this.DestinationListAsync(DestinationListRQ, null);
        }
        
        /// <remarks/>
        public void DestinationListAsync(string DestinationListRQ, object userState) {
            if ((this.DestinationListOperationCompleted == null)) {
                this.DestinationListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDestinationListOperationCompleted);
            }
            this.InvokeAsync("DestinationList", new object[] {
                        DestinationListRQ}, this.DestinationListOperationCompleted, userState);
        }
        
        private void OnDestinationListOperationCompleted(object arg) {
            if ((this.DestinationListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DestinationListCompleted(this, new DestinationListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("LocationList", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("LocationListRS", Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)]
        public string LocationList([System.Xml.Serialization.XmlElementAttribute(Namespace="http://ws.mph.nemogroup.net/", IsNullable=true)] string LocationListRQ) {
            object[] results = this.Invoke("LocationList", new object[] {
                        LocationListRQ});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void LocationListAsync(string LocationListRQ) {
            this.LocationListAsync(LocationListRQ, null);
        }
        
        /// <remarks/>
        public void LocationListAsync(string LocationListRQ, object userState) {
            if ((this.LocationListOperationCompleted == null)) {
                this.LocationListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLocationListOperationCompleted);
            }
            this.InvokeAsync("LocationList", new object[] {
                        LocationListRQ}, this.LocationListOperationCompleted, userState);
        }
        
        private void OnLocationListOperationCompleted(object arg) {
            if ((this.LocationListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LocationListCompleted(this, new LocationListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void HotelAvailCompletedEventHandler(object sender, HotelAvailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HotelAvailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HotelAvailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void HotelDescriptiveInfoCompletedEventHandler(object sender, HotelDescriptiveInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HotelDescriptiveInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HotelDescriptiveInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void HotelResCompletedEventHandler(object sender, HotelResCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HotelResCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HotelResCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void HotelBookingRuleCompletedEventHandler(object sender, HotelBookingRuleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HotelBookingRuleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HotelBookingRuleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void HotelResModifyCompletedEventHandler(object sender, HotelResModifyCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HotelResModifyCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HotelResModifyCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void QueryBookingCompletedEventHandler(object sender, QueryBookingCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class QueryBookingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal QueryBookingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void RateBreakDownCompletedEventHandler(object sender, RateBreakDownCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RateBreakDownCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RateBreakDownCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void ReadCompletedEventHandler(object sender, ReadCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ReadCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ReadCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void VoucherCompletedEventHandler(object sender, VoucherCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class VoucherCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal VoucherCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void DestinationListCompletedEventHandler(object sender, DestinationListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DestinationListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DestinationListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void LocationListCompletedEventHandler(object sender, LocationListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LocationListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal LocationListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591