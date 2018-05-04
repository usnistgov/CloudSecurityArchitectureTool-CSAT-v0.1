# Cloud Security Architecture Tool

## CSAT v1.0

### Implmentation and Deployment Issues
The original proof of concept was not designed with ease of installation or portability in mind. The rewrite aims to solve these problems:
* Reliance on Microsoft WPF for GUI components, which uses cryptographic functions that are not FIPS-140 approved or allowed.
* The use of MS SQL Express is unnecessary and cumbersome for a client application and adds too much complexity during installation that could be avoided by using a portable database such as SQLite.
* For spreadsheet operations, the previous implementation relied on Microsoft Excel Interop, which added complexity and overhead, as well as requiring users to have desktop versions of Excel to be installed on the machine.
* Limited to Windows because of reliance on C#, SQL Express and Excel Interop libraries.

## CSAT v2.0

### System Architectural Updates

CSRC v 2.0 is an updated proof of concept of the original Cloud Security Architecture Tool that aims to solve these issues. The entire engine is rewritten in Java to eliminate the dependency on Microsoft Windows. The User Interface (UI) is rewritten in Java FX. The application relies on the Apache POI libraries for spreadsheet operations and SQLite for storage.

### Implementation Status

Under Development. CSRC v2.0 is not complete. The project is only in the development stages. Many features are incomplete or buggy.

### NOTE

Although this proof of concept has a similar graphical design to the original proof of concept, we think that a CRSC can be greatly improved if implmented as a web tool that supports Identity, Credential and Access Management (ICAM) and multi-tenancy for multiple projects to co-exist.

## Description
Cloud Security Architecture Tool (CSAT), is a tool (proof of concept) that aims to leverage the Cybersecurity Framework (CSF) to identify the NIST SP 800-53 security and privacy controls for cloud-based information systems by identifying the necessary functional capabilities the system needs to provide to support the organization's mission and the service the system is designed for.

All federal agencies are charged and entrusted with safeguarding the information that is contained in their systems and with ensuring that these systems operate securely and reliably. To support federal agencies, NIST develops security and privacy risk management standards and guidelines that assist agencies in implementing integrated, organization-wide programs to manage information security risk as mandated by the Federal Information Systems Management Act (FISMA) of 2014 and May 2017 Presidential Executive Order on Strengthening the Cybersecurity of Federal Networks and Critical Infrastructure(link is external). Agencies manage many types of risk and develop specific policies to identify, assess, and help mitigate adverse effects across a wide range of risks, with cybersecurity among them. The latest broad adoption of cloud-based solution for the federal information systems has the potential to elevate the systems’ security posture when solutions are properly architected and implemented. The complexity of such systems makes though the risk assessment and the design of a proper solution more difficult and much harder to achieve, especially when the IT security professionals’ work is done in spreadsheets and documents driving paper-based compliance which is out-of-date the day after it’s written, and often leaving critical risks undiscovered or unaddressed. The U.S. Chief Information Officer (CIO) tasked the National Institute of Standards and Technology (NIST), along with other agencies, with specific activities aimed at accelerating the adoption of cloud computing. The tasks include the delivery of a Cloud Computing Security Reference Architecture (SP 500-299 – recently renumbered to SP 800-200). The document provides a methodology of architecting a cloud-based federal information. Additional guidance referred to as SP 800-174: “Security and Privacy Controls for Cloud-based Federal Information Systems”, leverages the methodology in SP 500-299/800-200 to provide exhaustive sets of SP 800-53 R4. security and privacy controls recommended for each functional capability or micro-service a system implements. Providing a user-friendly tool that leverages the NIST’s cloud computing security architecture (SP 800-200) and the questionnaire that drives the selection process of functional capabilities and associated security and privacy controls deemed necessary to be implemented for the information system to operate as indented with minimum residual risk (data aggregated in the SP 800-174) would support agencies in meeting the FISMA requirements and the Presidential Order mandates while creating a risk management dialog that facilitates the communication among agency stakeholders through the process of prioritization, implementation and integration of the security and privacy controls deemed necessary.  The tool referred to as Cloud Security Architecture Tool (CSAT), should also allow system information analysis (e.g. user should be able to select a control and identify all its instances and the capabilities with which the instances are associated document the implementation of the controls, the generation), and visualization of the system’s security posture.

The purpose of the project is to enhance and facilitate government agencies’ adoption of secure cloud solution through the development of the Cloud Security Architecture Tool (CSAT) – a tool that would draw on the exhaustive guidance NIST provides for the government agencies on how to secure information systems (cloud-based in particular) while meeting FISMA requirements regarding applying a risk based approach to securing information systems and the OMB mandate of using the NIST Cyber Security Framework for this purpose. CSRC would bring together guidance provide by NIST in: 
<li>
-- FIPS 199: “Standards for Security Categorization of Federal Information and Information Systems”http://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.199.pdf),
</li><li>
-- FISP 200: “Minimum Security Requirements for Federal Information and Information Systems” (http://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.200.pdf), 
</li><li>
-- SP 500-299/800-200: “NIST Cloud Computing Security Reference Architecture” (http://collaborate.nist.gov/twiki-cloud-computing/pub/CloudComputing/CloudSecurity/NIST_Security_Reference_Architecture_2013.05.15_v1.0.pdf), 
</li><li>
-- SP 800-53: “Security and Privacy Controls for Federal Information systems and Organizations” (http://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-53r4.pdf), 
</li><li>
-- SP 800-174: “Security and Privacy Controls for Cloud-based Federal Information Systems” (dataset only available herein at: TBD, 
</li><li>
-- NIST Cybersecurity Framework (v1.1) (https://www.nist.gov/sites/default/files/documents/draft-cybersecurity-framework-v1.11.pdf),  and
</li><li>
-- NISTIR 8170: “The Cybersecurity Framework. Implementation Guidance for Federal Agencies” (https://csrc.nist.gov/CSRC/media/Publications/nistir/8170/draft/documents/nistir8170-draft.pdf)
</li>

***
