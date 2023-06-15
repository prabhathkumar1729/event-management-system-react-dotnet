import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Sidebar from "./Sidebar";
import { Menu } from "@material-ui/icons";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";

export default function Navbar() {
  const [open, setOpen] = useState(false);
  const auth = useSelector(store => store.auth)
  const navigate = useNavigate();
  return (
    <>
    <Box sx={{ flexGrow: 1,position:'relative',zIndex:100 }}>
      <AppBar position="static" style={{ background: "#3f51b5" }}>
        <Toolbar>
          {auth.accessToken?
          
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{ mr: 2 }}
            onClick={() => {
              setOpen(!open);
            }}
          >
            <Menu />
          </IconButton>:<></>}

          <Typography variant="h6" className="BookMyEventLogo" component="div" sx={{ flexGrow: 1}} onClick={()=>{navigate("/admin");setOpen(false)}}>
            BookMyEvent
          </Typography>
        </Toolbar>
      </AppBar>
    </Box>
      <div style={{position:'fixed',zIndex:`${open?"50":"-100"}` }}>
        <Sidebar open={open} setOpen={setOpen} />
      </div>
      </>
  );
}
