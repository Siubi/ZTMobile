package example.com.ztmobile;

import android.content.Intent;
import android.support.annotation.IdRes;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.widget.Toolbar;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;
import android.os.Handler;

import com.google.android.gms.maps.CameraUpdate;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.model.LatLng;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    private Toolbar toolbar;
    private ActionBarDrawerToggle drawerToggle;
    private DrawerLayout drawerLayout;
    private ListView leftDrawer;
    private Fragment currentFragment;
    private boolean isBackKeyPressedTwice;
    private TrackingScreen trackingScreenFragment;
    private LoginScreen loginScreenFragment;
    private MapScreen mapScreenFragment;
    //private AccountScreen accountScreenFragment;
    //private DateTime lastBackButtonClickTime;

    List<String> leftMenuItems = new ArrayList<String>();
    ArrayAdapter leftMenuArrayAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        isBackKeyPressedTwice = false;
        toolbar = (Toolbar) findViewById(R.id.toolbar1);
        drawerLayout = (DrawerLayout) findViewById(R.id.drawer_layout);
        leftDrawer = (ListView) findViewById(R.id.left_drawer);

        leftMenuItems.add("Śledzenie");
        leftMenuItems.add("Konto");
        leftMenuItems.add("Mapa");
        leftMenuArrayAdapter = new ArrayAdapter(this, android.R.layout.simple_list_item_1, leftMenuItems);
        leftDrawer.setAdapter(leftMenuArrayAdapter);
        leftDrawer.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position,
                                    long id) {
                switch (position)
                {
                    case 0:
                        ShowFragment(trackingScreenFragment);
                        break;

                    case 1:
                        ShowFragment(loginScreenFragment);
                        break;

                    case 2:
                        if (FunctionsAndGlobals.isTrackingEnabled == true)
                        {
                            LatLng latLng = new LatLng(FunctionsAndGlobals.googleMap.getMyLocation().getLatitude(), FunctionsAndGlobals.googleMap.getMyLocation().getLongitude());
                            CameraUpdate cameraUpdate = CameraUpdateFactory.newLatLngZoom(latLng, 16);
                            FunctionsAndGlobals.googleMap.animateCamera(cameraUpdate);
                        }
                        ShowFragment(mapScreenFragment);
                        break;

                    default:
                        break;
                }

                drawerLayout.closeDrawer(GravityCompat.START);
            }
        });

        setSupportActionBar(toolbar);

        mapScreenFragment = new MapScreen();
        trackingScreenFragment = new TrackingScreen();
        loginScreenFragment = new LoginScreen();

        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        //transaction.add(Resource.Id.fragmentContainer, accountScreenFragment, "Account Screen");
        //transaction.hide(accountScreenFragment);
        transaction.add(R.id.fragmentContainer, mapScreenFragment, "Map Screen");
        transaction.hide(mapScreenFragment);
        //.add(Resource.Id.fragmentContainer, loginScreenFragment, "Login Screen");
        //transaction.hide(loginScreenFragment);
        transaction.add(R.id.fragmentContainer, trackingScreenFragment, "Tracking Screen");
        transaction.commit();

        currentFragment = trackingScreenFragment;

        drawerToggle = new ActionBarDrawerToggle(MainActivity.this, drawerLayout, R.string.openDrawer, R.string.closeDrawer);

        drawerLayout.setDrawerListener(drawerToggle);
        getSupportActionBar().setHomeButtonEnabled(true);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        getSupportActionBar().setDisplayShowTitleEnabled(true);
        drawerToggle.syncState();
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Pass the event to ActionBarDrawerToggle, if it returns
        // true, then it has handled the app icon touch event
        if (drawerToggle.onOptionsItemSelected(item)) {
            return true;
        }
        // Handle your other action bar items...

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onBackPressed()
    {
        if (isBackKeyPressedTwice == true)
        {
            Intent intent = new Intent(Intent.ACTION_MAIN);
            intent.addCategory(Intent.CATEGORY_HOME);
            intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
            startActivity(intent);
            finish();
            System.exit(0);
        }

        Toast.makeText(MainActivity.this, R.string.exitMessage, Toast.LENGTH_SHORT).show();

        new android.os.Handler().postDelayed(new Runnable() {
            @Override
            public void run() {
                isBackKeyPressedTwice = false;
            }
        }, FunctionsAndGlobals.doubleBackButtonClickInterval_ms);

        isBackKeyPressedTwice = true;
    }

    private void ShowFragment(Fragment fragment)
    {
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.hide(currentFragment);
        transaction.show(fragment);
        transaction.addToBackStack(null);
        transaction.commit();

        currentFragment = fragment;
    }
}
