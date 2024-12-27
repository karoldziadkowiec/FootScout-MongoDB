import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import ProtectedRoute from '../services/roles/ProtectedRoute';
import DynamicNavbar from '../components/layout/DynamicNavbar';
import Role from '../models/enums/Role';
import Login from '../components/account/Login';
import Registration from '../components/account/Registration';
import Home from '../components/home/Home';
import MyProfile from '../components/user/MyProfile';
import ClubHistory from '../components/user/ClubHistory';
import PlayerAdvertisements from '../components/playerAdvertisement/PlayerAdvertisements';
import ClubAdvertisements from '../components/clubAdvertisement/ClubAdvertisements';
import PlayerAdvertisement from '../components/playerAdvertisement/PlayerAdvertisement';
import ClubAdvertisement from '../components/clubAdvertisement/ClubAdvertisement';
import MyPlayerAdvertisements from '../components/user/MyPlayerAdvertisements';
import MyClubAdvertisements from '../components/user/MyClubAdvertisements';
import MyFavoritePlayerAdvertisements from '../components/user/MyFavoritePlayerAdvertisements';
import MyFavoriteClubAdvertisements from '../components/user/MyFavoriteClubAdvertisements';
import Chats from '../components/chat/Chats';
import Chat from '../components/chat/Chat';
import MyOffersAsPlayer from '../components/user/MyOffersAsPlayer';
import MyOffersAsClub from '../components/user/MyOffersAsClub';
import NewPlayerAdvertisement from '../components/playerAdvertisement/NewPlayerAdvertisement';
import NewClubAdvertisement from '../components/clubAdvertisement/NewClubAdvertisement';
import Support from '../components/support/Support';
import AdminDashboard from '../components/admin/AdminDashboard';
import AdminUsers from '../components/admin/AdminUsers';
import AdminChats from '../components/admin/AdminChats';
import AdminChat from '../components/admin/AdminChat';
import AdminPlayerAdvertisements from '../components/admin/AdminPlayerAdvertisements';
import AdminClubAdvertisements from '../components/admin/AdminClubAdvertisements';
import AdminPlayerOffers from '../components/admin/AdminPlayerOffers';
import AdminClubOffers from '../components/admin/AdminClubOffers';
import AdminSupport from '../components/admin/AdminSupport';
import AdminPlayerPositions from '../components/admin/AdminPlayerPositions';
import AdminMakeAnAdmin from '../components/admin/AdminMakeAnAdmin';
import AdminRaportsUsers from '../components/admin/AdminRaportsUsers';
import AdminRaportsChats from '../components/admin/AdminRaportsChats';
import AdminRaportsPlayerAdvertisements from '../components/admin/AdminRaportsPlayerAdvertisements';
import AdminRaportsClubAdvertisements from '../components/admin/AdminRaportsClubAdvertisements';
import AdminRaportsPlayerOffers from '../components/admin/AdminRaportsPlayerOffers';
import AdminRaportsClubOffers from '../components/admin/AdminRaportsClubOffers';

const Routing = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/registration" element={<Registration />} />
        <Route path="/home" element={<ProtectedRoute element={<DynamicNavbar><Home /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/my-profile" element={<ProtectedRoute element={<DynamicNavbar><MyProfile /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/club-history" element={<ProtectedRoute element={<DynamicNavbar><ClubHistory /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/player-advertisements" element={<ProtectedRoute element={<DynamicNavbar><PlayerAdvertisements /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/club-advertisements" element={<ProtectedRoute element={<DynamicNavbar><ClubAdvertisements /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/player-advertisement/:id" element={<ProtectedRoute element={<DynamicNavbar><PlayerAdvertisement /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/club-advertisement/:id" element={<ProtectedRoute element={<DynamicNavbar><ClubAdvertisement /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/my-player-advertisements" element={<ProtectedRoute element={<DynamicNavbar><MyPlayerAdvertisements /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/my-club-advertisements" element={<ProtectedRoute element={<DynamicNavbar><MyClubAdvertisements /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/my-favorite-player-advertisements" element={<ProtectedRoute element={<DynamicNavbar><MyFavoritePlayerAdvertisements /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/my-favorite-club-advertisements" element={<ProtectedRoute element={<DynamicNavbar><MyFavoriteClubAdvertisements /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/chats" element={<ProtectedRoute element={<DynamicNavbar><Chats /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/chat/:id" element={<ProtectedRoute element={<DynamicNavbar><Chat /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        <Route path="/my-offers-as-player" element={<ProtectedRoute element={<DynamicNavbar><MyOffersAsPlayer /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/my-offers-as-club" element={<ProtectedRoute element={<DynamicNavbar><MyOffersAsClub /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/new-player-advertisement" element={<ProtectedRoute element={<DynamicNavbar><NewPlayerAdvertisement /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/new-club-advertisement" element={<ProtectedRoute element={<DynamicNavbar><NewClubAdvertisement /></DynamicNavbar>} allowedRoles={[Role.User]} />} />
        <Route path="/support" element={<ProtectedRoute element={<DynamicNavbar><Support /></DynamicNavbar>} allowedRoles={[Role.Admin, Role.User]} />} />
        {/* Admin */}
        <Route path="/admin/dashboard" element={<ProtectedRoute element={<DynamicNavbar><AdminDashboard /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/users" element={<ProtectedRoute element={<DynamicNavbar><AdminUsers /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/chats" element={<ProtectedRoute element={<DynamicNavbar><AdminChats /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/chat/:id" element={<ProtectedRoute element={<DynamicNavbar><AdminChat /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/player-advertisements" element={<ProtectedRoute element={<DynamicNavbar><AdminPlayerAdvertisements /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/club-advertisements" element={<ProtectedRoute element={<DynamicNavbar><AdminClubAdvertisements /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/player-offers" element={<ProtectedRoute element={<DynamicNavbar><AdminPlayerOffers /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/club-offers" element={<ProtectedRoute element={<DynamicNavbar><AdminClubOffers /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/support" element={<ProtectedRoute element={<DynamicNavbar><AdminSupport /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/player-positions" element={<ProtectedRoute element={<DynamicNavbar><AdminPlayerPositions /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/make-admin" element={<ProtectedRoute element={<DynamicNavbar><AdminMakeAnAdmin /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/raports/users" element={<ProtectedRoute element={<DynamicNavbar><AdminRaportsUsers /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/raports/chats" element={<ProtectedRoute element={<DynamicNavbar><AdminRaportsChats /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/raports/player-advertisements" element={<ProtectedRoute element={<DynamicNavbar><AdminRaportsPlayerAdvertisements /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/raports/club-advertisements" element={<ProtectedRoute element={<DynamicNavbar><AdminRaportsClubAdvertisements /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/raports/player-offers" element={<ProtectedRoute element={<DynamicNavbar><AdminRaportsPlayerOffers /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
        <Route path="/admin/raports/club-offers" element={<ProtectedRoute element={<DynamicNavbar><AdminRaportsClubOffers /></DynamicNavbar>} allowedRoles={[Role.Admin]} />} />
      </Routes>
    </Router>
  );
};

export default Routing;