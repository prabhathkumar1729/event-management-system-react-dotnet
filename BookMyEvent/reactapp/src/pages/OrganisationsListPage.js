import React from 'react';
import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Card, CardContent, Typography, Grid } from '@mui/material';
import { fetchOrganisationById, fetchOrganisations } from '../Features/ReducerSlices/OrganisationsSlice';
import store from '../App/store';
import { makeStyles } from "@material-ui/core/styles";



const useStyles = makeStyles((theme) => ({
    card: {
      backgroundColor: theme.palette.background.default,
      borderRadius: theme.shape.borderRadius,
      boxShadow: theme.shadows[2],
      height: '100%',
    },
    cardContent: {
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'space-between',
      height: '100%',
    },
    organizationName: {
      fontWeight: 'bold',
      marginBottom: theme.spacing(1),
    },
    organizationLocation: {
      fontStyle: 'italic',
      color: theme.palette.text.secondary,
      marginBottom: theme.spacing(2),
    },
  }));
  
const OrganizationCard = ({ organization }) => {
    const classes = useStyles();
    return (
      <Card className={classes.card}>
        <CardContent className={classes.cardContent}>
            <div>
          <Typography variant="h5" className={classes.organizationName} >
            {organization.organisationName}
          </Typography>
          <Typography variant="body2" className={classes.organizationLocation}>
            {organization.location}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {organization.organisationDescription}
          </Typography>
          </div>
        </CardContent>
      </Card>
    );
  };
const OrganisationsListPage = () => {
    const dispatch = useDispatch();
    useEffect(() => {
        //dispatch(fetchOrganisations());
        dispatch(fetchOrganisationById("C934E5F9-B6B5-4D28-932C-F96C981AE70F"));
    }, [dispatch]);
    // var organisations = useSelector(store => store.organisations.organisations);
  
   var organisations=store.getState().organisations.organisations;
    console.log(organisations);



    return ( <>
     <Grid container spacing={2}>
      {organisations.map((organization) => (
        <Grid item xs={12} sm={6} md={4} key={organization.organisationId
        }>
          <OrganizationCard organization={organization} />
        </Grid>
      ))}
    </Grid>
    </>
        
    );
}

export default OrganisationsListPage;
