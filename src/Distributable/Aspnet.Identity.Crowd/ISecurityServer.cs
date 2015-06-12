using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.Remoting;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	// ReSharper disable InconsistentNaming
	public interface ISecurityServer
	{
		SoapProtocolVersion SoapVersion { get; set; }
		bool AllowAutoRedirect { get; set; }
		CookieContainer CookieContainer { get; set; }
		X509CertificateCollection ClientCertificates { get; }
		bool EnableDecompression { get; set; }
		string UserAgent { get; set; }
		IWebProxy Proxy { get; set; }
		bool UnsafeAuthenticatedConnectionSharing { get; set; }
		ICredentials Credentials { get; set; }
		bool UseDefaultCredentials { get; set; }
		string ConnectionGroupName { get; set; }
		bool PreAuthenticate { get; set; }
		string Url { get; set; }
		Encoding RequestEncoding { get; set; }
		int Timeout { get; set; }
		ISite Site { get; set; }
		IContainer Container { get; }

		/// <remarks />
		event findAllGroupRelationshipsCompletedEventHandler findAllGroupRelationshipsCompleted;

		/// <remarks />
		event addGroupCompletedEventHandler addGroupCompleted;

		/// <remarks />
		event addPrincipalToRoleCompletedEventHandler addPrincipalToRoleCompleted;

		/// <remarks />
		event findPrincipalByTokenCompletedEventHandler findPrincipalByTokenCompleted;

		/// <remarks />
		event updatePrincipalCredentialCompletedEventHandler updatePrincipalCredentialCompleted;

		/// <remarks />
		event getGrantedAuthoritiesCompletedEventHandler getGrantedAuthoritiesCompleted;

		/// <remarks />
		event addPrincipalCompletedEventHandler addPrincipalCompleted;

		/// <remarks />
		event addAttributeToPrincipalCompletedEventHandler addAttributeToPrincipalCompleted;

		/// <remarks />
		event invalidatePrincipalTokenCompletedEventHandler invalidatePrincipalTokenCompleted;

		/// <remarks />
		event findAllGroupNamesCompletedEventHandler findAllGroupNamesCompleted;

		/// <remarks />
		event findRoleMembershipsCompletedEventHandler findRoleMembershipsCompleted;

		/// <remarks />
		event removePrincipalCompletedEventHandler removePrincipalCompleted;

		/// <remarks />
		event isValidPrincipalTokenCompletedEventHandler isValidPrincipalTokenCompleted;

		/// <remarks />
		event authenticatePrincipalSimpleCompletedEventHandler authenticatePrincipalSimpleCompleted;

		/// <remarks />
		event removeRoleCompletedEventHandler removeRoleCompleted;

		/// <remarks />
		event getCookieInfoCompletedEventHandler getCookieInfoCompleted;

		/// <remarks />
		event updatePrincipalAttributeCompletedEventHandler updatePrincipalAttributeCompleted;

		/// <remarks />
		event searchGroupsCompletedEventHandler searchGroupsCompleted;

		/// <remarks />
		event getCacheTimeCompletedEventHandler getCacheTimeCompleted;

		/// <remarks />
		event isRoleMemberCompletedEventHandler isRoleMemberCompleted;

		/// <remarks />
		event updateGroupCompletedEventHandler updateGroupCompleted;

		/// <remarks />
		event addAttributeToGroupCompletedEventHandler addAttributeToGroupCompleted;

		/// <remarks />
		event findAllRoleNamesCompletedEventHandler findAllRoleNamesCompleted;

		/// <remarks />
		event findRoleByNameCompletedEventHandler findRoleByNameCompleted;

		/// <remarks />
		event isCacheEnabledCompletedEventHandler isCacheEnabledCompleted;

		/// <remarks />
		event findGroupByNameCompletedEventHandler findGroupByNameCompleted;

		/// <remarks />
		event findGroupWithAttributesByNameCompletedEventHandler findGroupWithAttributesByNameCompleted;

		/// <remarks />
		event removePrincipalFromRoleCompletedEventHandler removePrincipalFromRoleCompleted;

		/// <remarks />
		event findPrincipalWithAttributesByNameCompletedEventHandler findPrincipalWithAttributesByNameCompleted;

		/// <remarks />
		event authenticatePrincipalCompletedEventHandler authenticatePrincipalCompleted;

		/// <remarks />
		event findGroupMembershipsCompletedEventHandler findGroupMembershipsCompleted;

		/// <remarks />
		event addPrincipalToGroupCompletedEventHandler addPrincipalToGroupCompleted;

		/// <remarks />
		event removeGroupCompletedEventHandler removeGroupCompleted;

		/// <remarks />
		event removeAttributeFromGroupCompletedEventHandler removeAttributeFromGroupCompleted;

		/// <remarks />
		event addAllPrincipalsCompletedEventHandler addAllPrincipalsCompleted;

		/// <remarks />
		event removeAttributeFromPrincipalCompletedEventHandler removeAttributeFromPrincipalCompleted;

		/// <remarks />
		event addRoleCompletedEventHandler addRoleCompleted;

		/// <remarks />
		event findAllPrincipalNamesCompletedEventHandler findAllPrincipalNamesCompleted;

		/// <remarks />
		event createPrincipalTokenCompletedEventHandler createPrincipalTokenCompleted;

		/// <remarks />
		event searchRolesCompletedEventHandler searchRolesCompleted;

		/// <remarks />
		event removePrincipalFromGroupCompletedEventHandler removePrincipalFromGroupCompleted;

		/// <remarks />
		event findPrincipalByNameCompletedEventHandler findPrincipalByNameCompleted;

		/// <remarks />
		event resetPrincipalCredentialCompletedEventHandler resetPrincipalCredentialCompleted;

		/// <remarks />
		event updateGroupAttributeCompletedEventHandler updateGroupAttributeCompleted;

		/// <remarks />
		event isGroupMemberCompletedEventHandler isGroupMemberCompleted;

		/// <remarks />
		event searchPrincipalsCompletedEventHandler searchPrincipalsCompleted;

		/// <remarks />
		event getDomainCompletedEventHandler getDomainCompleted;

		/// <remarks />
		event authenticateApplicationCompletedEventHandler authenticateApplicationCompleted;

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		[return: XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")]
		SOAPNestableGroup[] findAllGroupRelationships([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BeginfindAllGroupRelationships(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPNestableGroup[] EndfindAllGroupRelationships(IAsyncResult asyncResult);

		/// <remarks />
		void findAllGroupRelationshipsAsync(AuthenticatedToken in0);

		/// <remarks />
		void findAllGroupRelationshipsAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPGroup addGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] SOAPGroup in1);

		/// <remarks />
		IAsyncResult BeginaddGroup(AuthenticatedToken in0, SOAPGroup in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPGroup EndaddGroup(IAsyncResult asyncResult);

		/// <remarks />
		void addGroupAsync(AuthenticatedToken in0, SOAPGroup in1);

		/// <remarks />
		void addGroupAsync(AuthenticatedToken in0, SOAPGroup in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void addPrincipalToRole([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginaddPrincipalToRole(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndaddPrincipalToRole(IAsyncResult asyncResult);

		/// <remarks />
		void addPrincipalToRoleAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void addPrincipalToRoleAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPPrincipal findPrincipalByToken([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindPrincipalByToken(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPPrincipal EndfindPrincipalByToken(IAsyncResult asyncResult);

		/// <remarks />
		void findPrincipalByTokenAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findPrincipalByTokenAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void updatePrincipalCredential([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] PasswordCredential in2);

		/// <remarks />
		IAsyncResult BeginupdatePrincipalCredential(AuthenticatedToken in0, string in1, PasswordCredential in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndupdatePrincipalCredential(IAsyncResult asyncResult);

		/// <remarks />
		void updatePrincipalCredentialAsync(AuthenticatedToken in0, string in1, PasswordCredential in2);

		/// <remarks />
		void updatePrincipalCredentialAsync(AuthenticatedToken in0, string in1, PasswordCredential in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		string[] getGrantedAuthorities([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BegingetGrantedAuthorities(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		string[] EndgetGrantedAuthorities(IAsyncResult asyncResult);

		/// <remarks />
		void getGrantedAuthoritiesAsync(AuthenticatedToken in0);

		/// <remarks />
		void getGrantedAuthoritiesAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPPrincipal addPrincipal([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] SOAPPrincipal in1, [XmlElement(IsNullable = true)] PasswordCredential in2);

		/// <remarks />
		IAsyncResult BeginaddPrincipal(AuthenticatedToken in0, SOAPPrincipal in1, PasswordCredential in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPPrincipal EndaddPrincipal(IAsyncResult asyncResult);

		/// <remarks />
		void addPrincipalAsync(AuthenticatedToken in0, SOAPPrincipal in1, PasswordCredential in2);

		/// <remarks />
		void addPrincipalAsync(AuthenticatedToken in0, SOAPPrincipal in1, PasswordCredential in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void addAttributeToPrincipal([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] SOAPAttribute in2);

		/// <remarks />
		IAsyncResult BeginaddAttributeToPrincipal(AuthenticatedToken in0, string in1, SOAPAttribute in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndaddAttributeToPrincipal(IAsyncResult asyncResult);

		/// <remarks />
		void addAttributeToPrincipalAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2);

		/// <remarks />
		void addAttributeToPrincipalAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void invalidatePrincipalToken([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BegininvalidatePrincipalToken(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndinvalidatePrincipalToken(IAsyncResult asyncResult);

		/// <remarks />
		void invalidatePrincipalTokenAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void invalidatePrincipalTokenAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		string[] findAllGroupNames([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BeginfindAllGroupNames(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		string[] EndfindAllGroupNames(IAsyncResult asyncResult);

		/// <remarks />
		void findAllGroupNamesAsync(AuthenticatedToken in0);

		/// <remarks />
		void findAllGroupNamesAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		string[] findRoleMemberships([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindRoleMemberships(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		string[] EndfindRoleMemberships(IAsyncResult asyncResult);

		/// <remarks />
		void findRoleMembershipsAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findRoleMembershipsAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removePrincipal([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginremovePrincipal(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremovePrincipal(IAsyncResult asyncResult);

		/// <remarks />
		void removePrincipalAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void removePrincipalAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out")]
		bool isValidPrincipalToken([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1,
			[XmlArray(IsNullable = true)] [XmlArrayItem(Namespace = "http://authentication.integration.crowd.atlassian.com")] ValidationFactor[] in2);

		/// <remarks />
		IAsyncResult BeginisValidPrincipalToken(AuthenticatedToken in0, string in1, ValidationFactor[] in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		bool EndisValidPrincipalToken(IAsyncResult asyncResult);

		/// <remarks />
		void isValidPrincipalTokenAsync(AuthenticatedToken in0, string in1, ValidationFactor[] in2);

		/// <remarks />
		void isValidPrincipalTokenAsync(AuthenticatedToken in0, string in1, ValidationFactor[] in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		string authenticatePrincipalSimple([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginauthenticatePrincipalSimple(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		string EndauthenticatePrincipalSimple(IAsyncResult asyncResult);

		/// <remarks />
		void authenticatePrincipalSimpleAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void authenticatePrincipalSimpleAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removeRole([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginremoveRole(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremoveRole(IAsyncResult asyncResult);

		/// <remarks />
		void removeRoleAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void removeRoleAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPCookieInfo getCookieInfo([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BegingetCookieInfo(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPCookieInfo EndgetCookieInfo(IAsyncResult asyncResult);

		/// <remarks />
		void getCookieInfoAsync(AuthenticatedToken in0);

		/// <remarks />
		void getCookieInfoAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void updatePrincipalAttribute([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] SOAPAttribute in2);

		/// <remarks />
		IAsyncResult BeginupdatePrincipalAttribute(AuthenticatedToken in0, string in1, SOAPAttribute in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndupdatePrincipalAttribute(IAsyncResult asyncResult);

		/// <remarks />
		void updatePrincipalAttributeAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2);

		/// <remarks />
		void updatePrincipalAttributeAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		[return: XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")]
		SOAPGroup[] searchGroups([XmlElement(IsNullable = true)] AuthenticatedToken in0,
			[XmlArray(IsNullable = true)] [XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")] SearchRestriction[] in1);

		/// <remarks />
		IAsyncResult BeginsearchGroups(AuthenticatedToken in0, SearchRestriction[] in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPGroup[] EndsearchGroups(IAsyncResult asyncResult);

		/// <remarks />
		void searchGroupsAsync(AuthenticatedToken in0, SearchRestriction[] in1);

		/// <remarks />
		void searchGroupsAsync(AuthenticatedToken in0, SearchRestriction[] in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out")]
		long getCacheTime([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BegingetCacheTime(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		long EndgetCacheTime(IAsyncResult asyncResult);

		/// <remarks />
		void getCacheTimeAsync(AuthenticatedToken in0);

		/// <remarks />
		void getCacheTimeAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out")]
		bool isRoleMember([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginisRoleMember(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		bool EndisRoleMember(IAsyncResult asyncResult);

		/// <remarks />
		void isRoleMemberAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void isRoleMemberAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void updateGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2, bool in3);

		/// <remarks />
		IAsyncResult BeginupdateGroup(AuthenticatedToken in0, string in1, string in2, bool in3, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndupdateGroup(IAsyncResult asyncResult);

		/// <remarks />
		void updateGroupAsync(AuthenticatedToken in0, string in1, string in2, bool in3);

		/// <remarks />
		void updateGroupAsync(AuthenticatedToken in0, string in1, string in2, bool in3, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void addAttributeToGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] SOAPAttribute in2);

		/// <remarks />
		IAsyncResult BeginaddAttributeToGroup(AuthenticatedToken in0, string in1, SOAPAttribute in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndaddAttributeToGroup(IAsyncResult asyncResult);

		/// <remarks />
		void addAttributeToGroupAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2);

		/// <remarks />
		void addAttributeToGroupAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		string[] findAllRoleNames([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BeginfindAllRoleNames(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		string[] EndfindAllRoleNames(IAsyncResult asyncResult);

		/// <remarks />
		void findAllRoleNamesAsync(AuthenticatedToken in0);

		/// <remarks />
		void findAllRoleNamesAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPRole findRoleByName([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindRoleByName(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPRole EndfindRoleByName(IAsyncResult asyncResult);

		/// <remarks />
		void findRoleByNameAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findRoleByNameAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out")]
		bool isCacheEnabled([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BeginisCacheEnabled(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		bool EndisCacheEnabled(IAsyncResult asyncResult);

		/// <remarks />
		void isCacheEnabledAsync(AuthenticatedToken in0);

		/// <remarks />
		void isCacheEnabledAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPGroup findGroupByName([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindGroupByName(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPGroup EndfindGroupByName(IAsyncResult asyncResult);

		/// <remarks />
		void findGroupByNameAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findGroupByNameAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPGroup findGroupWithAttributesByName([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindGroupWithAttributesByName(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPGroup EndfindGroupWithAttributesByName(IAsyncResult asyncResult);

		/// <remarks />
		void findGroupWithAttributesByNameAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findGroupWithAttributesByNameAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removePrincipalFromRole([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginremovePrincipalFromRole(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremovePrincipalFromRole(IAsyncResult asyncResult);

		/// <remarks />
		void removePrincipalFromRoleAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void removePrincipalFromRoleAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPPrincipal findPrincipalWithAttributesByName([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindPrincipalWithAttributesByName(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPPrincipal EndfindPrincipalWithAttributesByName(IAsyncResult asyncResult);

		/// <remarks />
		void findPrincipalWithAttributesByNameAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findPrincipalWithAttributesByNameAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		string authenticatePrincipal([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] UserAuthenticationContext in1);

		/// <remarks />
		IAsyncResult BeginauthenticatePrincipal(AuthenticatedToken in0, UserAuthenticationContext in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		string EndauthenticatePrincipal(IAsyncResult asyncResult);

		/// <remarks />
		void authenticatePrincipalAsync(AuthenticatedToken in0, UserAuthenticationContext in1);

		/// <remarks />
		void authenticatePrincipalAsync(AuthenticatedToken in0, UserAuthenticationContext in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		string[] findGroupMemberships([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindGroupMemberships(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		string[] EndfindGroupMemberships(IAsyncResult asyncResult);

		/// <remarks />
		void findGroupMembershipsAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findGroupMembershipsAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void addPrincipalToGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginaddPrincipalToGroup(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndaddPrincipalToGroup(IAsyncResult asyncResult);

		/// <remarks />
		void addPrincipalToGroupAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void addPrincipalToGroupAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removeGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginremoveGroup(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremoveGroup(IAsyncResult asyncResult);

		/// <remarks />
		void removeGroupAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void removeGroupAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removeAttributeFromGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginremoveAttributeFromGroup(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremoveAttributeFromGroup(IAsyncResult asyncResult);

		/// <remarks />
		void removeAttributeFromGroupAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void removeAttributeFromGroupAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void addAllPrincipals([XmlElement(IsNullable = true)] AuthenticatedToken in0,
			[XmlArray(IsNullable = true)] [XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")] SOAPPrincipalWithCredential[] in1);

		/// <remarks />
		IAsyncResult BeginaddAllPrincipals(AuthenticatedToken in0, SOAPPrincipalWithCredential[] in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndaddAllPrincipals(IAsyncResult asyncResult);

		/// <remarks />
		void addAllPrincipalsAsync(AuthenticatedToken in0, SOAPPrincipalWithCredential[] in1);

		/// <remarks />
		void addAllPrincipalsAsync(AuthenticatedToken in0, SOAPPrincipalWithCredential[] in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removeAttributeFromPrincipal([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginremoveAttributeFromPrincipal(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremoveAttributeFromPrincipal(IAsyncResult asyncResult);

		/// <remarks />
		void removeAttributeFromPrincipalAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void removeAttributeFromPrincipalAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPRole addRole([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] SOAPRole in1);

		/// <remarks />
		IAsyncResult BeginaddRole(AuthenticatedToken in0, SOAPRole in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPRole EndaddRole(IAsyncResult asyncResult);

		/// <remarks />
		void addRoleAsync(AuthenticatedToken in0, SOAPRole in1);

		/// <remarks />
		void addRoleAsync(AuthenticatedToken in0, SOAPRole in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		string[] findAllPrincipalNames([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BeginfindAllPrincipalNames(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		string[] EndfindAllPrincipalNames(IAsyncResult asyncResult);

		/// <remarks />
		void findAllPrincipalNamesAsync(AuthenticatedToken in0);

		/// <remarks />
		void findAllPrincipalNamesAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		string createPrincipalToken([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1,
			[XmlArray(IsNullable = true)] [XmlArrayItem(Namespace = "http://authentication.integration.crowd.atlassian.com")] ValidationFactor[] in2);

		/// <remarks />
		IAsyncResult BegincreatePrincipalToken(AuthenticatedToken in0, string in1, ValidationFactor[] in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		string EndcreatePrincipalToken(IAsyncResult asyncResult);

		/// <remarks />
		void createPrincipalTokenAsync(AuthenticatedToken in0, string in1, ValidationFactor[] in2);

		/// <remarks />
		void createPrincipalTokenAsync(AuthenticatedToken in0, string in1, ValidationFactor[] in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		[return: XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")]
		SOAPRole[] searchRoles([XmlElement(IsNullable = true)] AuthenticatedToken in0,
			[XmlArray(IsNullable = true)] [XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")] SearchRestriction[] in1);

		/// <remarks />
		IAsyncResult BeginsearchRoles(AuthenticatedToken in0, SearchRestriction[] in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPRole[] EndsearchRoles(IAsyncResult asyncResult);

		/// <remarks />
		void searchRolesAsync(AuthenticatedToken in0, SearchRestriction[] in1);

		/// <remarks />
		void searchRolesAsync(AuthenticatedToken in0, SearchRestriction[] in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void removePrincipalFromGroup([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginremovePrincipalFromGroup(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndremovePrincipalFromGroup(IAsyncResult asyncResult);

		/// <remarks />
		void removePrincipalFromGroupAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void removePrincipalFromGroupAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		SOAPPrincipal findPrincipalByName([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginfindPrincipalByName(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPPrincipal EndfindPrincipalByName(IAsyncResult asyncResult);

		/// <remarks />
		void findPrincipalByNameAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void findPrincipalByNameAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void resetPrincipalCredential([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1);

		/// <remarks />
		IAsyncResult BeginresetPrincipalCredential(AuthenticatedToken in0, string in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndresetPrincipalCredential(IAsyncResult asyncResult);

		/// <remarks />
		void resetPrincipalCredentialAsync(AuthenticatedToken in0, string in1);

		/// <remarks />
		void resetPrincipalCredentialAsync(AuthenticatedToken in0, string in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		void updateGroupAttribute([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] SOAPAttribute in2);

		/// <remarks />
		IAsyncResult BeginupdateGroupAttribute(AuthenticatedToken in0, string in1, SOAPAttribute in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		void EndupdateGroupAttribute(IAsyncResult asyncResult);

		/// <remarks />
		void updateGroupAttributeAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2);

		/// <remarks />
		void updateGroupAttributeAsync(AuthenticatedToken in0, string in1, SOAPAttribute in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out")]
		bool isGroupMember([XmlElement(IsNullable = true)] AuthenticatedToken in0, [XmlElement(IsNullable = true)] string in1, [XmlElement(IsNullable = true)] string in2);

		/// <remarks />
		IAsyncResult BeginisGroupMember(AuthenticatedToken in0, string in1, string in2, AsyncCallback callback, object asyncState);

		/// <remarks />
		bool EndisGroupMember(IAsyncResult asyncResult);

		/// <remarks />
		void isGroupMemberAsync(AuthenticatedToken in0, string in1, string in2);

		/// <remarks />
		void isGroupMemberAsync(AuthenticatedToken in0, string in1, string in2, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("out", IsNullable = true)]
		[return: XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")]
		SOAPPrincipal[] searchPrincipals([XmlElement(IsNullable = true)] AuthenticatedToken in0,
			[XmlArray(IsNullable = true)] [XmlArrayItem(Namespace = "http://soap.integration.crowd.atlassian.com")] SearchRestriction[] in1);

		/// <remarks />
		IAsyncResult BeginsearchPrincipals(AuthenticatedToken in0, SearchRestriction[] in1, AsyncCallback callback, object asyncState);

		/// <remarks />
		SOAPPrincipal[] EndsearchPrincipals(IAsyncResult asyncResult);

		/// <remarks />
		void searchPrincipalsAsync(AuthenticatedToken in0, SearchRestriction[] in1);

		/// <remarks />
		void searchPrincipalsAsync(AuthenticatedToken in0, SearchRestriction[] in1, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		string getDomain([XmlElement(IsNullable = true)] AuthenticatedToken in0);

		/// <remarks />
		IAsyncResult BegingetDomain(AuthenticatedToken in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		string EndgetDomain(IAsyncResult asyncResult);

		/// <remarks />
		void getDomainAsync(AuthenticatedToken in0);

		/// <remarks />
		void getDomainAsync(AuthenticatedToken in0, object userState);

		/// <remarks />
		[SoapDocumentMethod("", RequestNamespace = "urn:SecurityServer", ResponseNamespace = "urn:SecurityServer", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("out", IsNullable = true)]
		AuthenticatedToken authenticateApplication([XmlElement(IsNullable = true)] ApplicationAuthenticationContext in0);

		/// <remarks />
		IAsyncResult BeginauthenticateApplication(ApplicationAuthenticationContext in0, AsyncCallback callback, object asyncState);

		/// <remarks />
		AuthenticatedToken EndauthenticateApplication(IAsyncResult asyncResult);

		/// <remarks />
		void authenticateApplicationAsync(ApplicationAuthenticationContext in0);

		/// <remarks />
		void authenticateApplicationAsync(ApplicationAuthenticationContext in0, object userState);

		/// <remarks />
		void CancelAsync(object userState);

		void Discover();

		void Abort();

		void Dispose();

		string ToString();

		event EventHandler Disposed;

		object GetLifetimeService();

		object InitializeLifetimeService();

		ObjRef CreateObjRef(Type requestedType);
	}
}