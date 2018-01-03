package gov.nist.csrk;

import gov.nist.csrk.spreadsheet.UpdateDB;
import gov.nist.csrk.ui.MainWindow;
import javafx.application.Application;
import org.apache.commons.cli.*;
import org.apache.log4j.BasicConfigurator;
import org.apache.log4j.Logger;
import org.jooq.DSLContext;
import org.jooq.SQLDialect;
import org.jooq.impl.DSL;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.Arrays;
import java.util.prefs.BackingStoreException;
import java.util.prefs.Preferences;

/**
 * Created by naw2 on 12/22/2017.
 */
public class CloudSecurityRubiksCube {
    private static final Logger log = Logger.getLogger(MainWindow.class.getName());

    public static void main(String args[]) {
        BasicConfigurator.configure(); // TODO use a decent logging configuration
        // Display fancy ASCII logo
        System.out.println("                                      _ _ _\n" +
                "                                     /_/_/_/\\\n" +
                "                                    /_/_/_/\\/\\\n" +
                "                                   /_/_/_/\\/\\/\\\n" +
                "                                   \\_\\_\\_\\/\\/\\/\n" +
                "                                    \\_\\_\\_\\/\\/\n" +
                "                                     \\_\\_\\_\\/\n" +
                "    _   _________________   ____        __    _ __  _          ______      __       \n" +
                "   / | / /  _/ ___/_  __/  / __ \\__  __/ /_  (_/ /_( )_____   / ______  __/ /_  ___ \n" +
                "  /  |/ // / \\__ \\ / /    / /_/ / / / / __ \\/ / //_|// ___/  / /   / / / / __ \\/ _ \\\n" +
                " / /|  _/ / ___/ // /    / _, _/ /_/ / /_/ / / ,<   (__  )  / /___/ /_/ / /_/ /  __/\n" +
                "/_/ |_/___//____//_/    /_/ |_|\\__,_/_.___/_/_/|_| /____/   \\____/\\__,_/_.___/\\___/\n");
        // NOTE gui uses preferences while cli does not
        if(args.length > 0) {
            System.out.println("Running in cli... Parsing args");

            Options options = new Options();

            Option dbFile = new Option("db", "dbFile", true,"SQLite database path");
            dbFile.setRequired(true);
            options.addOption(dbFile);

            Option implem3Col = new Option("3col", "using 3 column implementation");
            options.addOption(implem3Col);

            CommandLineParser parser = new DefaultParser();
            HelpFormatter formatter = new HelpFormatter();
            CommandLine cmd;

            switch (args[0]) {
                case "update": {
                    args = args.length == 1 ? new String[0] : Arrays.copyOfRange(args, 1, args.length - 1); // remove first arg
                    Option capabilities = new Option("cap", "capabilities", true,
                            "capabilities excel sheet path");
                    capabilities.setRequired(true);
                    options.addOption(capabilities);

                    Option controls = new Option("con", "controls", true,
                            "controls excel sheet path (may be the same as capabilities)");
                    controls.setRequired(true);
                    options.addOption(controls);

                    Option baselines = new Option("base", "baselineSecurityMappings", true,
                            "baseline security mappings excel sheet path");
                    baselines.setRequired(true);
                    options.addOption(baselines);

                    try {
                        cmd = parser.parse(options, args);
                    } catch (ParseException e) {
                        formatter.printHelp("csrk update", options);
                        System.exit(1);
                        return;
                    }

                    String dbPath = cmd.getOptionValue("db");
                    String capabilitiesPath = cmd.getOptionValue("cap");
                    String controlsPath = cmd.getOptionValue("con");
                    String baselinesPath = cmd.getOptionValue("base");

                    Connection con = null;
                    try {
                        con = DriverManager.getConnection("jdbc:sqlite:" + dbPath);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }
                    DSLContext context = DSL.using(con, SQLDialect.SQLITE);

                    UpdateDB updateDB = new UpdateDB(context);

                    if (cmd.hasOption("3col")) {
                        updateDB.setImplementation3Col(true);
                        log.info("Running in 3 column mode");
                    }

                    updateDB.updateCapabilities(capabilitiesPath);
                    updateDB.updateControls(controlsPath);
                    updateDB.updateBaselineSecurityMappings(baselinesPath);
                    break;
                } case "reset_preferences": {
                    Preferences prefs = Preferences.userNodeForPackage(gov.nist.csrk.CloudSecurityRubiksCube.class);
                    try {
                        prefs.clear();
                    } catch (BackingStoreException e) {
                        e.printStackTrace();
                    }
                    System.out.println("Preferences reset");
                    break;
                } default: {
                    System.out.println("'" + args[0] + "' is not a valid argument, please use update or reset_preferences");
                }
            }
        } else {
            Application.launch(MainWindow.class, args);
        }
    }
}
