﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Testing.ProductionASMX {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TextPortSMSMessages", Namespace="http://www.textport.com/WebServices/")]
    [System.SerializableAttribute()]
    public partial class TextPortSMSMessages : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<Testing.ProductionASMX.TextPortSMSMessage> MessagesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string UserName {
            get {
                return this.UserNameField;
            }
            set {
                if ((object.ReferenceEquals(this.UserNameField, value) != true)) {
                    this.UserNameField = value;
                    this.RaisePropertyChanged("UserName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public System.Collections.Generic.List<Testing.ProductionASMX.TextPortSMSMessage> Messages {
            get {
                return this.MessagesField;
            }
            set {
                if ((object.ReferenceEquals(this.MessagesField, value) != true)) {
                    this.MessagesField = value;
                    this.RaisePropertyChanged("Messages");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TextPortSMSMessage", Namespace="http://www.textport.com/WebServices/")]
    [System.SerializableAttribute()]
    public partial class TextPortSMSMessage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CountryCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MobileNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageTextField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string CountryCode {
            get {
                return this.CountryCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.CountryCodeField, value) != true)) {
                    this.CountryCodeField = value;
                    this.RaisePropertyChanged("CountryCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string MobileNumber {
            get {
                return this.MobileNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.MobileNumberField, value) != true)) {
                    this.MobileNumberField = value;
                    this.RaisePropertyChanged("MobileNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string MessageText {
            get {
                return this.MessageTextField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageTextField, value) != true)) {
                    this.MessageTextField = value;
                    this.RaisePropertyChanged("MessageText");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TextPortSMSResponses", Namespace="http://www.textport.com/WebServices/")]
    [System.SerializableAttribute()]
    public partial class TextPortSMSResponses : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<Testing.ProductionASMX.TextPortSMSResponse> ResponsesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public System.Collections.Generic.List<Testing.ProductionASMX.TextPortSMSResponse> Responses {
            get {
                return this.ResponsesField;
            }
            set {
                if ((object.ReferenceEquals(this.ResponsesField, value) != true)) {
                    this.ResponsesField = value;
                    this.RaisePropertyChanged("Responses");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TextPortSMSResponse", Namespace="http://www.textport.com/WebServices/")]
    [System.SerializableAttribute()]
    public partial class TextPortSMSResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int ItemNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MobileNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResultField;
        
        private int MessageIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ProcessingMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int ItemNumber {
            get {
                return this.ItemNumberField;
            }
            set {
                if ((this.ItemNumberField.Equals(value) != true)) {
                    this.ItemNumberField = value;
                    this.RaisePropertyChanged("ItemNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string MobileNumber {
            get {
                return this.MobileNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.MobileNumberField, value) != true)) {
                    this.MobileNumberField = value;
                    this.RaisePropertyChanged("MobileNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Result {
            get {
                return this.ResultField;
            }
            set {
                if ((object.ReferenceEquals(this.ResultField, value) != true)) {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public int MessageID {
            get {
                return this.MessageIDField;
            }
            set {
                if ((this.MessageIDField.Equals(value) != true)) {
                    this.MessageIDField = value;
                    this.RaisePropertyChanged("MessageID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string ProcessingMessage {
            get {
                return this.ProcessingMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ProcessingMessageField, value) != true)) {
                    this.ProcessingMessageField = value;
                    this.RaisePropertyChanged("ProcessingMessage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string ErrorMessage {
            get {
                return this.ErrorMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true)) {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.textport.com/WebServices/", ConfigurationName="ProductionASMX.SMSClientSoap")]
    public interface SMSClientSoap {
        
        // CODEGEN: Generating message contract since element name PingResult from namespace http://www.textport.com/WebServices/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/Ping", ReplyAction="*")]
        Testing.ProductionASMX.PingResponse Ping(Testing.ProductionASMX.PingRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/Ping", ReplyAction="*")]
        System.Threading.Tasks.Task<Testing.ProductionASMX.PingResponse> PingAsync(Testing.ProductionASMX.PingRequest request);
        
        // CODEGEN: Generating message contract since element name userName from namespace http://www.textport.com/WebServices/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/VerifyAuthentication", ReplyAction="*")]
        Testing.ProductionASMX.VerifyAuthenticationResponse VerifyAuthentication(Testing.ProductionASMX.VerifyAuthenticationRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/VerifyAuthentication", ReplyAction="*")]
        System.Threading.Tasks.Task<Testing.ProductionASMX.VerifyAuthenticationResponse> VerifyAuthenticationAsync(Testing.ProductionASMX.VerifyAuthenticationRequest request);
        
        // CODEGEN: Generating message contract since element name messagesList from namespace http://www.textport.com/WebServices/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/SendMessages", ReplyAction="*")]
        Testing.ProductionASMX.SendMessagesResponse SendMessages(Testing.ProductionASMX.SendMessagesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/SendMessages", ReplyAction="*")]
        System.Threading.Tasks.Task<Testing.ProductionASMX.SendMessagesResponse> SendMessagesAsync(Testing.ProductionASMX.SendMessagesRequest request);
        
        // CODEGEN: Generating message contract since element name userName from namespace http://www.textport.com/WebServices/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/GetCreditBalance", ReplyAction="*")]
        Testing.ProductionASMX.GetCreditBalanceResponse GetCreditBalance(Testing.ProductionASMX.GetCreditBalanceRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.textport.com/WebServices/GetCreditBalance", ReplyAction="*")]
        System.Threading.Tasks.Task<Testing.ProductionASMX.GetCreditBalanceResponse> GetCreditBalanceAsync(Testing.ProductionASMX.GetCreditBalanceRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PingRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Ping", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.PingRequestBody Body;
        
        public PingRequest() {
        }
        
        public PingRequest(Testing.ProductionASMX.PingRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class PingRequestBody {
        
        public PingRequestBody() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PingResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PingResponse", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.PingResponseBody Body;
        
        public PingResponse() {
        }
        
        public PingResponse(Testing.ProductionASMX.PingResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class PingResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string PingResult;
        
        public PingResponseBody() {
        }
        
        public PingResponseBody(string PingResult) {
            this.PingResult = PingResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class VerifyAuthenticationRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="VerifyAuthentication", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.VerifyAuthenticationRequestBody Body;
        
        public VerifyAuthenticationRequest() {
        }
        
        public VerifyAuthenticationRequest(Testing.ProductionASMX.VerifyAuthenticationRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class VerifyAuthenticationRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string userName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        public VerifyAuthenticationRequestBody() {
        }
        
        public VerifyAuthenticationRequestBody(string userName, string password) {
            this.userName = userName;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class VerifyAuthenticationResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="VerifyAuthenticationResponse", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.VerifyAuthenticationResponseBody Body;
        
        public VerifyAuthenticationResponse() {
        }
        
        public VerifyAuthenticationResponse(Testing.ProductionASMX.VerifyAuthenticationResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class VerifyAuthenticationResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string VerifyAuthenticationResult;
        
        public VerifyAuthenticationResponseBody() {
        }
        
        public VerifyAuthenticationResponseBody(string VerifyAuthenticationResult) {
            this.VerifyAuthenticationResult = VerifyAuthenticationResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SendMessagesRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SendMessages", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.SendMessagesRequestBody Body;
        
        public SendMessagesRequest() {
        }
        
        public SendMessagesRequest(Testing.ProductionASMX.SendMessagesRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class SendMessagesRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Testing.ProductionASMX.TextPortSMSMessages messagesList;
        
        public SendMessagesRequestBody() {
        }
        
        public SendMessagesRequestBody(Testing.ProductionASMX.TextPortSMSMessages messagesList) {
            this.messagesList = messagesList;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SendMessagesResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SendMessagesResponse", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.SendMessagesResponseBody Body;
        
        public SendMessagesResponse() {
        }
        
        public SendMessagesResponse(Testing.ProductionASMX.SendMessagesResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class SendMessagesResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Testing.ProductionASMX.TextPortSMSResponses SendMessagesResult;
        
        public SendMessagesResponseBody() {
        }
        
        public SendMessagesResponseBody(Testing.ProductionASMX.TextPortSMSResponses SendMessagesResult) {
            this.SendMessagesResult = SendMessagesResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCreditBalanceRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCreditBalance", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.GetCreditBalanceRequestBody Body;
        
        public GetCreditBalanceRequest() {
        }
        
        public GetCreditBalanceRequest(Testing.ProductionASMX.GetCreditBalanceRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class GetCreditBalanceRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string userName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        public GetCreditBalanceRequestBody() {
        }
        
        public GetCreditBalanceRequestBody(string userName, string password) {
            this.userName = userName;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCreditBalanceResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCreditBalanceResponse", Namespace="http://www.textport.com/WebServices/", Order=0)]
        public Testing.ProductionASMX.GetCreditBalanceResponseBody Body;
        
        public GetCreditBalanceResponse() {
        }
        
        public GetCreditBalanceResponse(Testing.ProductionASMX.GetCreditBalanceResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.textport.com/WebServices/")]
    public partial class GetCreditBalanceResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int GetCreditBalanceResult;
        
        public GetCreditBalanceResponseBody() {
        }
        
        public GetCreditBalanceResponseBody(int GetCreditBalanceResult) {
            this.GetCreditBalanceResult = GetCreditBalanceResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SMSClientSoapChannel : Testing.ProductionASMX.SMSClientSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SMSClientSoapClient : System.ServiceModel.ClientBase<Testing.ProductionASMX.SMSClientSoap>, Testing.ProductionASMX.SMSClientSoap {
        
        public SMSClientSoapClient() {
        }
        
        public SMSClientSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SMSClientSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SMSClientSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SMSClientSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Testing.ProductionASMX.PingResponse Testing.ProductionASMX.SMSClientSoap.Ping(Testing.ProductionASMX.PingRequest request) {
            return base.Channel.Ping(request);
        }
        
        public string Ping() {
            Testing.ProductionASMX.PingRequest inValue = new Testing.ProductionASMX.PingRequest();
            inValue.Body = new Testing.ProductionASMX.PingRequestBody();
            Testing.ProductionASMX.PingResponse retVal = ((Testing.ProductionASMX.SMSClientSoap)(this)).Ping(inValue);
            return retVal.Body.PingResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Testing.ProductionASMX.PingResponse> Testing.ProductionASMX.SMSClientSoap.PingAsync(Testing.ProductionASMX.PingRequest request) {
            return base.Channel.PingAsync(request);
        }
        
        public System.Threading.Tasks.Task<Testing.ProductionASMX.PingResponse> PingAsync() {
            Testing.ProductionASMX.PingRequest inValue = new Testing.ProductionASMX.PingRequest();
            inValue.Body = new Testing.ProductionASMX.PingRequestBody();
            return ((Testing.ProductionASMX.SMSClientSoap)(this)).PingAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Testing.ProductionASMX.VerifyAuthenticationResponse Testing.ProductionASMX.SMSClientSoap.VerifyAuthentication(Testing.ProductionASMX.VerifyAuthenticationRequest request) {
            return base.Channel.VerifyAuthentication(request);
        }
        
        public string VerifyAuthentication(string userName, string password) {
            Testing.ProductionASMX.VerifyAuthenticationRequest inValue = new Testing.ProductionASMX.VerifyAuthenticationRequest();
            inValue.Body = new Testing.ProductionASMX.VerifyAuthenticationRequestBody();
            inValue.Body.userName = userName;
            inValue.Body.password = password;
            Testing.ProductionASMX.VerifyAuthenticationResponse retVal = ((Testing.ProductionASMX.SMSClientSoap)(this)).VerifyAuthentication(inValue);
            return retVal.Body.VerifyAuthenticationResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Testing.ProductionASMX.VerifyAuthenticationResponse> Testing.ProductionASMX.SMSClientSoap.VerifyAuthenticationAsync(Testing.ProductionASMX.VerifyAuthenticationRequest request) {
            return base.Channel.VerifyAuthenticationAsync(request);
        }
        
        public System.Threading.Tasks.Task<Testing.ProductionASMX.VerifyAuthenticationResponse> VerifyAuthenticationAsync(string userName, string password) {
            Testing.ProductionASMX.VerifyAuthenticationRequest inValue = new Testing.ProductionASMX.VerifyAuthenticationRequest();
            inValue.Body = new Testing.ProductionASMX.VerifyAuthenticationRequestBody();
            inValue.Body.userName = userName;
            inValue.Body.password = password;
            return ((Testing.ProductionASMX.SMSClientSoap)(this)).VerifyAuthenticationAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Testing.ProductionASMX.SendMessagesResponse Testing.ProductionASMX.SMSClientSoap.SendMessages(Testing.ProductionASMX.SendMessagesRequest request) {
            return base.Channel.SendMessages(request);
        }
        
        public Testing.ProductionASMX.TextPortSMSResponses SendMessages(Testing.ProductionASMX.TextPortSMSMessages messagesList) {
            Testing.ProductionASMX.SendMessagesRequest inValue = new Testing.ProductionASMX.SendMessagesRequest();
            inValue.Body = new Testing.ProductionASMX.SendMessagesRequestBody();
            inValue.Body.messagesList = messagesList;
            Testing.ProductionASMX.SendMessagesResponse retVal = ((Testing.ProductionASMX.SMSClientSoap)(this)).SendMessages(inValue);
            return retVal.Body.SendMessagesResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Testing.ProductionASMX.SendMessagesResponse> Testing.ProductionASMX.SMSClientSoap.SendMessagesAsync(Testing.ProductionASMX.SendMessagesRequest request) {
            return base.Channel.SendMessagesAsync(request);
        }
        
        public System.Threading.Tasks.Task<Testing.ProductionASMX.SendMessagesResponse> SendMessagesAsync(Testing.ProductionASMX.TextPortSMSMessages messagesList) {
            Testing.ProductionASMX.SendMessagesRequest inValue = new Testing.ProductionASMX.SendMessagesRequest();
            inValue.Body = new Testing.ProductionASMX.SendMessagesRequestBody();
            inValue.Body.messagesList = messagesList;
            return ((Testing.ProductionASMX.SMSClientSoap)(this)).SendMessagesAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Testing.ProductionASMX.GetCreditBalanceResponse Testing.ProductionASMX.SMSClientSoap.GetCreditBalance(Testing.ProductionASMX.GetCreditBalanceRequest request) {
            return base.Channel.GetCreditBalance(request);
        }
        
        public int GetCreditBalance(string userName, string password) {
            Testing.ProductionASMX.GetCreditBalanceRequest inValue = new Testing.ProductionASMX.GetCreditBalanceRequest();
            inValue.Body = new Testing.ProductionASMX.GetCreditBalanceRequestBody();
            inValue.Body.userName = userName;
            inValue.Body.password = password;
            Testing.ProductionASMX.GetCreditBalanceResponse retVal = ((Testing.ProductionASMX.SMSClientSoap)(this)).GetCreditBalance(inValue);
            return retVal.Body.GetCreditBalanceResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Testing.ProductionASMX.GetCreditBalanceResponse> Testing.ProductionASMX.SMSClientSoap.GetCreditBalanceAsync(Testing.ProductionASMX.GetCreditBalanceRequest request) {
            return base.Channel.GetCreditBalanceAsync(request);
        }
        
        public System.Threading.Tasks.Task<Testing.ProductionASMX.GetCreditBalanceResponse> GetCreditBalanceAsync(string userName, string password) {
            Testing.ProductionASMX.GetCreditBalanceRequest inValue = new Testing.ProductionASMX.GetCreditBalanceRequest();
            inValue.Body = new Testing.ProductionASMX.GetCreditBalanceRequestBody();
            inValue.Body.userName = userName;
            inValue.Body.password = password;
            return ((Testing.ProductionASMX.SMSClientSoap)(this)).GetCreditBalanceAsync(inValue);
        }
    }
}
