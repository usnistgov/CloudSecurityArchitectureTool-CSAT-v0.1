package gov.nist.csrk.ui;

import gov.nist.csrk.spreadsheet.UpdateDB;
import javafx.application.Application;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;

import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.image.Image;
import javafx.stage.DirectoryChooser;
import javafx.stage.Stage;
import org.apache.log4j.Logger;
import org.codehaus.plexus.util.FileUtils;
import org.jooq.DSLContext;
import org.jooq.SQLDialect;
import org.jooq.impl.DSL;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.prefs.Preferences;

/**
 * Created by naw2 on 12/22/2017.
 */
public class MainWindow extends Application {
    private static final Logger log = Logger.getLogger(MainWindow.class.getName());

    private Preferences prefs;
    private DSLContext context;
    private UpdateDB updateDB;

    private Stage stage;

    private void promptSetup() {
        while(true) {
            Alert alert = new Alert(Alert.AlertType.INFORMATION);
            alert.setTitle("First time setup");
            alert.setHeaderText("Welcome to the NIST Cloud Security Rubik's Cube");
            alert.setContentText("On the next screen you will be instructed to set a home folder for generated " +
                    "reports and database files");
            alert.showAndWait();

            DirectoryChooser directoryChooser = new DirectoryChooser();
            File selectedDirectory = directoryChooser.showDialog(stage);

            if(selectedDirectory == null) {
                System.exit(1); // user cancelled, abort
            } else if(!selectedDirectory.canWrite()) {
                alert = new Alert(Alert.AlertType.ERROR);
                alert.setTitle("First time setup - Directory setup error");
                alert.setContentText("Cannot write to selected directory '" + selectedDirectory.getAbsolutePath()
                        + "'.\n\nPlease chose another directory");
                alert.showAndWait();
            } else {
                prefs.put("workdir", selectedDirectory.getAbsolutePath());
                return;
            }
        }
    }

    private void firstTimeSetup() {
        log.info("Performing first time setup");
        promptSetup();
        File workdir = new File(prefs.get("workdir", ""));

        // copy internal database
        URL inputUrl = getClass().getResource("/internaldb.sqlite");
        File dest = new File(workdir.getAbsolutePath() + "\\database.sqlite");
        log.debug("Copying internal database to " + dest.getAbsolutePath());

        try {
            FileUtils.copyURLToFile(inputUrl, dest);
        } catch (IOException e) {
            log.error("Could not copy internal database to specified destination", e);
        } // TODO better exception handling here

        prefs.put("dbPath", dest.getAbsolutePath());
        prefs.putBoolean("firstUse", false);
    }

    /**
     * Called on Application.launch()
     */
    public void start(Stage stage) {
        this.stage = stage;

        FXMLLoader loader = new FXMLLoader(getClass().getResource("/ui/MainWindow.fxml"));
        loader.setController(this);
        Parent root = null;
        try {
            root = loader.load();
        } catch (IOException e) {
            log.error("Could not load fxml file from classpath", e);
            return;
        }
        Scene scene = new Scene(root);
        stage.setTitle("Cloud Security Rubik's Cube");
        stage.getIcons().add(new Image(getClass().getResourceAsStream("/ui/graphics/csrk.ico")));
        stage.setResizable(false);
        stage.setScene(scene);

        prefs = Preferences.userNodeForPackage(gov.nist.csrk.CloudSecurityRubiksCube.class);

        if(prefs.getBoolean("firstUse", true)) {
            firstTimeSetup();
        }

        // setup SQL connection and JOOQ context
        Connection con = null;
        try {
            con = DriverManager.getConnection("jdbc:sqlite:" + prefs.get("dbPath", "../../internaldb.sqlite"));
        } catch (SQLException e) {
            e.printStackTrace();
        }
        context = DSL.using(con, SQLDialect.SQLITE);
        updateDB = new UpdateDB(context);

        stage.show();

        if(!updateDB.isComplete()) {
            setDbOperatorsEnabled(false);
            Alert alert = new Alert(Alert.AlertType.ERROR);
            alert.setTitle("Database integrity error");
            alert.setHeaderText(null);
            alert.setContentText("Provided SQL Database is not complete! Please click 'Update Database' for more" +
                    " information");
            alert.showAndWait();
        }
    }

    @FXML private Button btnCapabilities;
    @FXML private Button btnControls;
    @FXML private Button btnBaselines;
    @FXML private Button btnTic;
    @FXML private Button btnVisualization;
    @FXML private Button btnUpdate;

    private void setDbOperatorsEnabled(boolean isEnabled) {
        log.debug((isEnabled ? "Enabling " : "Disabling ") + "report generation buttons");
        btnCapabilities.setDisable(!isEnabled);
        btnControls.setDisable(!isEnabled);
        btnBaselines.setDisable(!isEnabled);
        btnTic.setDisable(!isEnabled);
    }

    /**
     * Called after corresponding fxml file has been loaded
     */
    @FXML private void initialize() {
        log.debug("Initializing main window");
    }

    @FXML private void btnCapabilitiesClicked(ActionEvent event) {

    }

    @FXML private void btnControlsClicked(ActionEvent event) {

    }

    @FXML private void btnBaselinesClicked(ActionEvent event) {

    }

    @FXML private void btnTicClicked(ActionEvent event) {

    }

    @FXML private void btnVisualizationClicked(ActionEvent event) {

    }

    @FXML private void btnUpdateClicked(ActionEvent event) {
        UpdateWindow updateWindow = new UpdateWindow(updateDB, prefs);
        updateWindow.start();
        setDbOperatorsEnabled(updateDB.isComplete());
    }
}
