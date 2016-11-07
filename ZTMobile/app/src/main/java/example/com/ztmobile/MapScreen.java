package example.com.ztmobile;

import android.app.Activity;
import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.media.session.MediaControllerCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.google.android.gms.cast.CastRemoteDisplayLocalService;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.MapView;
import com.google.android.gms.maps.OnMapReadyCallback;


public class MapScreen extends Fragment {

    private MapView mapView;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        View view = inflater.inflate(R.layout.fragment_map_screen, container, false);

        //SetUp Map
        if (FunctionsAndGlobals.googleMap == null)
        {
            mapView = (MapView)view.findViewById(R.id.map);
            mapView.onCreate(savedInstanceState);
            mapView.onResume();
            mapView.getMapAsync(new OnMapReadyCallback() {
                @Override
                public void onMapReady(GoogleMap googleMap) {
                    if (googleMap != null) {
                        FunctionsAndGlobals.googleMap = googleMap;
                    }
                };
            });
        }

        return view;
    }

    //public void OnMapReady(GoogleMap map)
    //{
    //    FunctionsAndGlobals.googleMap = map;
    //}
}
