package gov.nist.csrk.ui;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;

import javafx.stage.Stage;

/**
 * Created by naw2 on 12/22/2017.
 */
public class MainWindow extends Application {
    public void start(Stage stage) throws Exception {
        Parent root = FXMLLoader.load(getClass().getResource("/ui/MainWindow.fxml"));
        Scene scene = new Scene(root);
        stage.setTitle("Cloud Security Rubik's Cube");
        stage.setScene(scene);

        stage.show();
    }
}
