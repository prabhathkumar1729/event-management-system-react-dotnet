import React, { useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Drawer, IconButton, List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { Menu as MenuIcon, ChevronLeft as ChevronLeftIcon, Inbox as InboxIcon,ExitToApp, GroupAdd, AccountCircle} from '@material-ui/icons';
import { useNavigate } from 'react-router-dom';
import useLogout from '../Hooks/useLogout';
import { useSelector } from 'react-redux';


const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
  },
  appBar: {
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
  },
  appBarShift: {
    width: `calc(100% - ${drawerWidth}px)`,
    marginLeft: drawerWidth,
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  menuButton: {
    marginRight: theme.spacing(2),
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
    background:'#f5f5f5',
    backdropFilter:'blur(6px)'
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    marginLeft: 0,
  },
}));

function Sidebar({open,setOpen}) {
  const navigate = useNavigate();
  const auth = useSelector(store => store.auth);
  const logout = useLogout();
  const classes = useStyles();
//   const [open, setOpen] = useState(false);

  const handleDrawerOpen = () => {
    setOpen(true);
  };

  const handleDrawerClose = () => {
    setOpen(false);
  };
  const handleLogout = async() =>{
      if(await logout()){
        handleDrawerClose();
        navigate('admin/login');
        handleDrawerClose();
      }
  }
  const handleProfile = async() =>{
    if(auth.role == "Admin"){
      navigate("/admin/profile");
      handleDrawerClose();
    }
  }
  const handleAddAdmin = async() =>{
    navigate("/admin/addadmin");
    handleDrawerClose();
  }
  return (
    <div className={classes.root}>
      <Drawer
        className={classes.drawer}
        variant="persistent"
        anchor="left"
        open={open}
        classes={{
          paper: classes.drawerPaper,
        }}
      >
        <div className={classes.drawerHeader}>
          <IconButton onClick={handleDrawerClose}>
            <ChevronLeftIcon />
          </IconButton>
        </div>
        <List>
        <ListItem button onClick={handleProfile}>
            <ListItemIcon>
              <AccountCircle/>
            </ListItemIcon>
            <ListItemText primary="Profile" />
          </ListItem>
          {auth.role === "Admin"?
          
        <ListItem button onClick={handleAddAdmin}>
            <ListItemIcon>
              <GroupAdd/>
            </ListItemIcon>
            <ListItemText primary="Add Admin" />
          </ListItem>:<></>}
          
          <ListItem button onClick={handleLogout}>
            <ListItemIcon>
              <ExitToApp/>
            </ListItemIcon>
            <ListItemText primary="Logout" />
          </ListItem>
        </List>
      </Drawer>
      <main className={classes.content}>
        <div className={classes.drawerHeader} />
        {/* Content */}
      </main>
    </div>
  );
}

export default Sidebar;
