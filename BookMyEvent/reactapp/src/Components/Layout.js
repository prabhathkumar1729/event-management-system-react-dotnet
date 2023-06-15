import React from 'react'
import Navbar from './Navbar'
import { Outlet, useLoaderData } from 'react-router-dom'
import Sidebar from './Sidebar'


const Layout = () => {
  return (
    <div>
      <Navbar/>
      <Outlet/>
    </div>
  )
}

export default Layout
