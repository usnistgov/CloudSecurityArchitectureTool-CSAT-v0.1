# Cloud Security Architecture Tool

## Description
Cloud Security Architecture Tool (CSAT), is a tool (proof of concept) that aims to leverage the Cybersecurity Framework (CSF) to identify the NIST SP 800-53 security and privacy controls for cloud-based information systems by identifying the necessary functional capabilities the system needs to provide to support the organization's mission and the service the system is designed for.

All federal agencies are charged and entrusted with safeguarding the information that is contained in their systems and with ensuring that these systems operate securely and reliably. To support federal agencies, NIST develops security and privacy risk management standards and guidelines that assist agencies in implementing integrated, organization-wide programs to manage information security risk as mandated by the Federal Information Systems Management Act (FISMA) of 2014 and May 2017 Presidential Executive Order on Strengthening the Cybersecurity of Federal Networks and Critical Infrastructure(link is external). Agencies manage many types of risk and develop specific policies to identify, assess, and help mitigate adverse effects across a wide range of risks, with cybersecurity among them. The latest broad adoption of cloud-based solution for the federal information systems has the potential to elevate the systems’ security posture when solutions are properly architected and implemented. The complexity of such systems makes though the risk assessment and the design of a proper solution more difficult and much harder to achieve, especially when the IT security professionals’ work is done in spreadsheets and documents driving paper-based compliance which is out-of-date the day after it’s written, and often leaving critical risks undiscovered or unaddressed. The U.S. Chief Information Officer (CIO) tasked the National Institute of Standards and Technology (NIST), along with other agencies, with specific activities aimed at accelerating the adoption of cloud computing. The tasks include the delivery of a Cloud Computing Security Reference Architecture (SP 500-299 – recently renumbered to SP 800-200). The document provides a methodology of architecting a cloud-based federal information. Additional guidance referred to as SP 800-174: “Security and Privacy Controls for Cloud-based Federal Information Systems”, leverages the methodology in SP 500-299/800-200 to provide exhaustive sets of SP 800-53 R4. security and privacy controls recommended for each functional capability or micro-service a system implements. Providing a user-friendly tool that leverages the NIST’s cloud computing security architecture (SP 800-200) and the questionnaire that drives the selection process of functional capabilities and associated security and privacy controls deemed necessary to be implemented for the information system to operate as indented with minimum residual risk (data aggregated in the SP 800-174) would support agencies in meeting the FISMA requirements and the Presidential Order mandates while creating a risk management dialog that facilitates the communication among agency stakeholders through the process of prioritization, implementation and integration of the security and privacy controls deemed necessary.  The tool referred to as Cloud Security Rubik’s Cube (CSRC), should also allow system information analysis (e.g. user should be able to select a control and identify all its instances and the capabilities with which the instances are associated document the implementation of the controls, the generation), and visualization of the system’s security posture.

The purpose of the project is to enhance and facilitate government agencies’ adoption of secure cloud solution through the development of the Cloud Security Rubik’s Cube (CSRC) – a tool that would draw on the exhaustive guidance NIST provides for the government agencies on how to secure information systems (cloud-based in particular) while meeting FISMA requirements regarding applying a risk based approach to securing information systems and the OMB mandate of using the NIST Cyber Security Framework for this purpose. For additional information, please also review the "Documents" directory https://github.com/usnistgov/CloudSecurityRubiksCube/tree/master/Documents. 
Core data of the CSRC and the proposed approach are captured in the Excel spreadsheet https://github.com/usnistgov/CloudSecurityRubiksCube/blob/master/Documents/CC_Overlay-SRA-RubiksCube_2017.08.17.xlsx. 
An overview and possible enhancements are provided in the two pdf files/presentations: <li>https://github.com/usnistgov/CloudSecurityRubiksCube/blob/master/Documents/Part-1_RiskManagementFramework_%26_CloudSecurityRubiksCube.pdf,</li<li> https://github.com/usnistgov/CloudSecurityRubiksCube/blob/master/Documents/Part-2_CloudSecurityRubiksCube%26OpenSecurityControlsAssessmentLanguage.pdf.</li>

Cloud Security Architecture Tool (CSAT) brings together guidance provide by NIST in the following documents: 
<li>
-- FIPS 199: “Standards for Security Categorization of Federal Information and Information Systems”http://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.199.pdf),
</li><li>
-- FISP 200: “Minimum Security Requirements for Federal Information and Information Systems” (http://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.200.pdf), 
</li><li>
-- SP 500-299/800-200: “NIST Cloud Computing Security Reference Architecture” (http://collaborate.nist.gov/twiki-cloud-computing/pub/CloudComputing/CloudSecurity/NIST_Security_Reference_Architecture_2013.05.15_v1.0.pdf), 
</li><li>
-- SP 800-53: “Security and Privacy Controls for Federal Information systems and Organizations” (http://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-53r4.pdf), 
</li><li>
-- NIST Cybersecurity Framework (v1.1) (https://www.nist.gov/sites/default/files/documents////draft-cybersecurity-framework-v1.11.pdf),  and
</li><li>
-- NISTIR 8170: “The Cybersecurity Framework. Implementation Guidance for Federal Agencies” (https://csrc.nist.gov/CSRC/media/Publications/nistir/8170/draft/documents/nistir8170-draft.pdf)
</li>

***
## Installation

### Disclaimer
This application uses Microsoft WPF which uses cryptographic functions that are not FIPS-140 approved or allowed. See #1

### Procedure
Currently, the installation for this program is as follows:
1. Install the appropriate version of MS SQL Server Express (w/ Management Studio). Make sure to set up a local database. In local tests we used MS SQL Express 2014.
2. Set up the database:
..1 Using the SQL Management Studio, run the schemaInit.sql script located in `ModelDB\schemaInit.sql`. You may need to run this script as an administrator.
..2 Again using the SQL Management Studio, run the dataInit.sql script located in `ModelDB\dataInit.sql`.
4. Install the CSRC application.
5. Run the CSRC application. The application should automatically detect local MS SQL servers.

Please note that this version of the application requires Microsoft Excel to generate reports or update the model. 

The CSRC application should be installed now

#### Installation Notes
<li>1. Please ensure that the connection string is working correctly for your setup of the SQL Server. SQL Express allows to read the machine name from registry. However, the SQL Server requires the instance name for the connection string, and possibly port number. The easiest thing is to pull out a connection string sample from the SQL Server Instance of the database you have set up for the Rubik’s Cube and use it in the code instead of using the existing one in teh installation package.
</li><li>
2. If Visual Stusio 2010 Pro has [SQL Server] or [Data Management] console, one can try to compose the connection string right from the console inside of the VS2010 to the SQL Server instance.
Please note, our project was developed using the free SQL Express which associates DB instance name with the machine name. If another version (not SQL Express) of SQL Server is used, you might need to comment out the logic that builds connection string fro SQL Express. The logic currently resides in the file DataConnecter.cs lines the lines 30-31. The variables conectstr and serversection are using executing machine name (line 29 - variable CompName), which would be completely wrong in the case of an external DB server.</li>
