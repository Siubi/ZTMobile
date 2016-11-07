package example.com.ztmobile;

import android.*;
import android.content.Context;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Bundle;
import android.support.annotation.MainThread;
import android.support.annotation.NonNull;
import android.support.annotation.UiThread;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

public class TrackingScreen extends Fragment {

    private Button buttonTrackingTrace;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_tracking_screen, container, false);

        buttonTrackingTrace = (Button) view.findViewById(R.id.buttonTrackTrace);
        FunctionsAndGlobals.isTrackingEnabled = false;

        buttonTrackingTrace.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                if (FunctionsAndGlobals.isTrackingEnabled == false)
                {
                    if(checkPermission()) {
                        buttonTrackingTrace.setText("Stop");
                        buttonTrackingTrace.setBackgroundResource(R.drawable.rounded_button_stop);
                        FunctionsAndGlobals.isTrackingEnabled = true;
                        FunctionsAndGlobals.googleMap.setMyLocationEnabled(true);
                    }
                    else askPermission();
                }
                else
                {
                    buttonTrackingTrace.setText("Start");
                    buttonTrackingTrace.setBackgroundResource(R.drawable.rounded_button);
                    FunctionsAndGlobals.isTrackingEnabled = false;
                    FunctionsAndGlobals.googleMap.setMyLocationEnabled(false);
                }
            }
        });

        return view;
    }

    private boolean checkPermission() {
        // Ask for permission if it wasn't granted yet
        return (ContextCompat.checkSelfPermission(getActivity(), android.Manifest.permission.ACCESS_FINE_LOCATION)
                == PackageManager.PERMISSION_GRANTED );
    }
    // Asks for permission
    private void askPermission() {
        ActivityCompat.requestPermissions(
                getActivity(),
                new String[] { android.Manifest.permission.ACCESS_FINE_LOCATION },
                1
        );
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        switch (requestCode) {
            case 1: {
                if (grantResults.length > 0
                        && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    // Permission granted
                    if (checkPermission()){
                        buttonTrackingTrace.setText("Stop");
                        buttonTrackingTrace.setBackgroundResource(R.drawable.rounded_button_stop);
                        FunctionsAndGlobals.isTrackingEnabled = true;
                        FunctionsAndGlobals.googleMap.setMyLocationEnabled(true);
                    }

                } else {
                    // Permission denied

                }
                break;
            }
        }
    }
}
