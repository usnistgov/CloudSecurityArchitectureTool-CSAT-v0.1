package gov.nist.csrc.ui;

import gov.nist.csrc.spreadsheet.UpdateDB;
import javafx.concurrent.Task;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;
import javafx.scene.control.TextArea;
import javafx.stage.FileChooser;
import javafx.stage.Modality;
import javafx.stage.Stage;
import org.apache.log4j.Logger;

import java.io.File;
import java.io.IOException;
import java.util.concurrent.CompletableFuture;
import java.util.prefs.Preferences;


public class UpdateWindow {
    private static final Logger log = Logger.getLogger(MainWindow.class.getName());

    private final UpdateDB updateDB;
    private final Stage stage;
    private final FileChooser fileChooser;

    UpdateWindow(UpdateDB updateDB, Preferences prefs) {
        this.updateDB = updateDB;

        fileChooser = new FileChooser();
        fileChooser.setInitialDirectory(new File(prefs.get("workdir", "")));

        stage = new Stage();
        FXMLLoader loader = new FXMLLoader(getClass().getResource("/ui/UpdateWindow.fxml"));
        loader.setController(this);
        Parent root = null;
        try {
            root = loader.load();
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }
        Scene scene = new Scene(root);
        stage.setTitle("CSRK Updater");
        stage.setResizable(false);
        stage.setScene(scene);
        stage.initModality(Modality.APPLICATION_MODAL);
    }

    void start() {
        stage.showAndWait();
    }

    @FXML private TextArea eventLog;
    @FXML private Button btnUpdateBase;
    @FXML private Button btnUpdateCons;
    @FXML private Button btnUpdateCaps;
    @FXML private Label operationLabel;
    @FXML private ProgressBar progress;

    @FXML private void initialize() {
        log.debug("Initializing update window");
    }

    @FXML private void btnUpdateCapsClicked(ActionEvent event) {
        File selectedFile = fileChooser.showOpenDialog(stage);
        if(selectedFile != null) {
            //CompletableFuture.runAsync(() -> updateDB.updateCapabilities(selectedFile.getAbsolutePath())).whenComplete(this::handleFutureTask);
            updateDB.updateCapabilities(selectedFile.getAbsolutePath());
        }
    }


    @FXML private void btnUpdateConsClicked(ActionEvent event) {
        File selectedFile = fileChooser.showOpenDialog(stage);
        if(selectedFile != null) {
            updateDB.updateControls(selectedFile.getAbsolutePath());
        }
    }

    @FXML private void btnUpdateBaseClicked(ActionEvent event) {
        File selectedFile = fileChooser.showOpenDialog(stage);
        if(selectedFile != null) {
            updateDB.updateBaselineSecurityMappings(selectedFile.getAbsolutePath());
        }
    }
}
