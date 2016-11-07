package example.com.ztmobile;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v7.widget.ContentFrameLayout;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.util.Random;

public class LoginScreen extends Fragment {

    private FragmentActivity myContext;
    private Button buttonSignUp;
    private Button buttonSignIn;
    private ProgressBar progressBar;
    private TextView txtGuestName;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_login_screen, container, false);

        Random rnd = new Random();

        txtGuestName = (TextView) view.findViewById(R.id.txtGuestName);
        progressBar = (ProgressBar) view.findViewById(R.id.progressBarLoginScreen);
        buttonSignUp = (Button) view.findViewById(R.id.buttonSignUp);
        buttonSignIn = (Button) view.findViewById(R.id.buttonSignIn);

        //generate random ID for guest
        txtGuestName.setText(((String) ("Guest" + (rnd.nextInt(89999) + 10000))));

        buttonSignIn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

            }
        });

        buttonSignUp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

            }
        });

        return view;
    }
}
