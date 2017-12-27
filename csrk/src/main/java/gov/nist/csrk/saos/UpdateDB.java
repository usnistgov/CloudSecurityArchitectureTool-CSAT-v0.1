package gov.nist.csrk.saos;

import gov.nist.csrk.jooq.tables.daos.*;
import gov.nist.csrk.jooq.tables.pojos.Baselinesecuritymappings;
import gov.nist.csrk.jooq.tables.pojos.Capabilities;
import gov.nist.csrk.jooq.tables.pojos.Controls;
import gov.nist.csrk.jooq.tables.pojos.Ticmappings;
import gov.nist.csrk.jooq.tables.records.BaselinesecuritymappingsRecord;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.openxml4j.opc.OPCPackage;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.jooq.DSLContext;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.regex.Pattern;

/**
 * Created by naw2 on 12/22/2017.
 */
public class UpdateDB {
    private final DSLContext context;
    private final BaselinesecuritymappingsDao baselinesecuritymappingsDao;
    private final ControlsDao controlsDao;
    private final SpecsDao specsDao;
    private final CapabilitiesDao capabilitiesDao;
    private final TicmappingsDao ticmappingsDao;
    private final MaptypescapabilitiescontrolsDao maptypescapabilitiescontrolsDao;

    UpdateDB(DSLContext context) {
        this.context = context;

        baselinesecuritymappingsDao = new BaselinesecuritymappingsDao(context.configuration());
        controlsDao = new ControlsDao(context.configuration());
        specsDao = new SpecsDao(context.configuration());
        capabilitiesDao = new CapabilitiesDao(context.configuration());
        ticmappingsDao = new TicmappingsDao(context.configuration());
        maptypescapabilitiescontrolsDao = new MaptypescapabilitiescontrolsDao(context.configuration());
    }

    /**
     * Opens Excel sheet for reading
     * @param path Path to excel sheet
     * @param position Workbook to return
     * @return the requested XSSFWorkbook
     */
    private XSSFSheet openSheet(String path, int position) {
        XSSFWorkbook workbook;
        try {
            InputStream inputStream = new FileInputStream(path);
            OPCPackage pkg = OPCPackage.open(inputStream);
            workbook = new XSSFWorkbook(pkg);
            pkg.close();
        } catch (InvalidFormatException e) {
            e.printStackTrace();
            return null;
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }

        return workbook.getSheetAt(position);
    }

    /**
     * Update Capabilities, TIC Mappings and MapTypeCapabilitiesControls
     * @param path to workbook
     * @return if successful in opening sheet
     */
    boolean updateCapabilities(String path) {
        XSSFSheet sheet = openSheet(path, 1);

        if(sheet == null)
            return false;

        // process tic capabilities and tic mappings
        List<Capabilities> capabilities = new ArrayList<Capabilities>();
        HashMap<String, Ticmappings> ticmappings = new HashMap<String, Ticmappings>();
        for(int i = 3; i < sheet.getPhysicalNumberOfRows(); i++) {
            XSSFRow row = sheet.getRow(i);
            Capabilities capability = new Capabilities();
            capability.setDomain(row.getCell(0).getStringCellValue());
            capability.setContainer(row.getCell(1).getStringCellValue());
            capability.setCapability(row.getCell(2).getStringCellValue());
            capability.setCapability2(row.getCell(3).getStringCellValue());
            capability.setDescription(row.getCell(4).getStringCellValue());
            capability.setCsadescription(""/*row.getCell(5).getStringCellValue()*/); // TODO ???
            String uniqueId = row.getCell(5).getStringCellValue();
            capability.setUniqueid(uniqueId);
            capability.setScopes(row.getCell(6).getStringCellValue());

            capability.setC((int) row.getCell(23).getNumericCellValue());
            capability.setI((int) row.getCell(24).getNumericCellValue());
            capability.setA((int) row.getCell(25).getNumericCellValue());

            capability.setResponsibilityvector(row.getCell(27).getStringCellValue() + "," +
                    row.getCell(31).getStringCellValue() + "," +
                    row.getCell(28).getStringCellValue() + "," +
                    row.getCell(32).getStringCellValue() + "," +
                    row.getCell(29).getStringCellValue() + "," +
                    row.getCell(33).getStringCellValue());
            capability.setOtheractors(row.getCell(39).getStringCellValue() + "," +
                    row.getCell(40).getStringCellValue() + "," +
                    row.getCell(41).getStringCellValue() + "," +
                    row.getCell(43).getStringCellValue() + "," +
                    row.getCell(45).getStringCellValue());

            String ticMappingsString = row.getCell(7).getStringCellValue();
            String[] entries = ticMappingsString.split("[;\n,]");
            for(String ticCap:entries) {
                Ticmappings ticData = new Ticmappings();
                ticData.setTicname(ticCap);
                ticmappings.put(uniqueId, ticData);
            }

            capabilities.add(capability);
        }
        capabilitiesDao.insert(capabilities);

        // find correct capabilitiesId for each ticMapping
        List<Ticmappings> ticList = new ArrayList<Ticmappings>();
        for(String uid:ticmappings.keySet()) {
            ticmappings.get(uid).setCapabilityid(capabilitiesDao.fetchByUniqueid(uid).get(0).getId());
            ticList.add(ticmappings.get(uid));
        }
        ticmappingsDao.insert(ticList);

        // process maptypes
        for(int i = 3; i < sheet.getPhysicalNumberOfRows(); i++) {
            XSSFRow row = sheet.getRow(i);

            int capId = capabilitiesDao.fetchByUniqueid(row.getCell(5).getStringCellValue()).get(0).getId();

            List<String> implemenationList = new ArrayList<String>();
            for(int level = 1; level <= 7; level++) {

            }
        }

        return true;
    }

    /**
     * Updates Controls, Specs, Relateds
     * @param path
     * @return
     */
    boolean updateControls(String path) {
        XSSFSheet sheet = openSheet(path, 2);

        if(sheet == null)
            return false;

        for(int i = 1; i < sheet.getPhysicalNumberOfRows(); i++) {
            XSSFRow row = sheet.getRow(i);
            // process row
        }

        return true;
    }

    private final int COL_NIST_LOW = 2;
    private final int COL_FED_LOW = 3;
    private final int COL_NIST_MED = 5;
    private final int COL_FED_MED = 6;
    private final int COL_NIST_HIGH = 7;
    private final int COL_FED_HIGH = 8;

    private final int LEVEL_LOW = 1;
    private final int LEVEL_MED = 2;
    private final int LEVEL_HIGH = 3;

    private final int AUTHOR_NIST = 1;
    private final int AUTHOR_FEDRAMP = 2;

    /**
     * Update BaselineSecurityMappings table
     *
     * Requires controls and specs to be up to date
     * @param path to workbook
     * @return if successful in opening sheet
     */
    boolean updateBaselineSecurityMappings(String path) {
        XSSFSheet sheet = openSheet(path, 0);

        if(sheet == null)
            return false;

        for(int i = 1; i < sheet.getPhysicalNumberOfRows(); i++) {
            XSSFRow row = sheet.getRow(i);
            if(row.getPhysicalNumberOfCells() > 0) {
                insertBaselineSecurityMapping(row.getCell(COL_NIST_LOW).getStringCellValue(), LEVEL_LOW, AUTHOR_NIST);
                insertBaselineSecurityMapping(row.getCell(COL_FED_LOW).getStringCellValue(), LEVEL_LOW, AUTHOR_FEDRAMP);
                insertBaselineSecurityMapping(row.getCell(COL_NIST_MED).getStringCellValue(), LEVEL_MED, AUTHOR_NIST);
                insertBaselineSecurityMapping(row.getCell(COL_FED_MED).getStringCellValue(), LEVEL_MED, AUTHOR_FEDRAMP);
                insertBaselineSecurityMapping(row.getCell(COL_NIST_HIGH).getStringCellValue(), LEVEL_HIGH, AUTHOR_NIST);
                insertBaselineSecurityMapping(row.getCell(COL_FED_HIGH).getStringCellValue(), LEVEL_HIGH, AUTHOR_FEDRAMP);
            }
        }

        return true;
    }

    /**
     * Insert new record into BaselineSecurityMappings with string containing many controls
     * @param component String containing many controls (parsed out with regex)
     * @param level (low, medium, or high corresponding to 1, 2 or 3)
     * @param author (1 denotes NIST, 2 denotes FEDRAMP)
     * @return false if control or spec could not be found
     */
    private boolean insertBaselineSecurityMapping(String component, int level, int author) {
        String[] controls = (String[]) getControlList(component).toArray();
        for(String entry:controls) {
            boolean isControlMap = Pattern.matches("[A-Z]{2}-([0-9]{1,2})", entry);
            int specsId = 1;
            int controlsId = 1;
            if(isControlMap) {
                List<Controls> filteredControls = controlsDao.fetchByName(entry);
                if(filteredControls.size() >= 1) {
                    controlsId = filteredControls.get(0).getId();
                } else {
                    // TODO throw an error or something
                    return false;
                }
            } else {
                // TODO get specs
            }

            Baselinesecuritymappings baseline = new Baselinesecuritymappings(
                    -1, level, author, isControlMap, specsId, controlsId);
//            baseline.setIscontrolmap(isControlMap);
//            baseline.setBaselineauthor(author);
//            baseline.setLevel(level);
//            baseline.setSpecsid(specsId);
//            baseline.setControlsid(controlsId);
            BaselinesecuritymappingsRecord newRecord = context.newRecord(
                    gov.nist.csrk.jooq.tables.Baselinesecuritymappings.BASELINESECURITYMAPPINGS, baseline);
            newRecord.store();
        }
        return true;
    }

    private List<String> getControlList(String rawString) {
        String[] rawControls = rawString.split("([,;\\n\\t *\\[\\]\\{\\}])");

        List<String> controls = new ArrayList<String>();
        for(String potentialControl:rawControls) {
            if(Pattern.matches("[A-Z]{2}-([0-9]{1,2})(\\((\\d|\\d\\d)\\)|)?", potentialControl)) {
                controls.add(potentialControl);
            } else {
                System.out.println("Malformed control: Pattern mismatch for " + potentialControl);
            }
        }
        return controls;
    }
}
