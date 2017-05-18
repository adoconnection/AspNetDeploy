import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';
import {IndexLinkContainer} from 'react-router-bootstrap';

import {Navbar, Nav, NavItem, MenuItem, NavDropdown} from 'react-bootstrap';

let TopPanel = ({router}) => (
    <Navbar inverse collapseOnSelect>
        <Navbar.Header>
            <Navbar.Brand>
                <IndexLinkContainer to="/">
                    <NavItem>ASP.NET Deploy</NavItem>
                </IndexLinkContainer>
            </Navbar.Brand>
            <Navbar.Toggle />
        </Navbar.Header>
        <Navbar.Collapse>
            <Nav>
                <NavItem eventKey={1} href="/App/Sources/List">Sources</NavItem>
            </Nav>
        </Navbar.Collapse>
    </Navbar>
);

TopPanel = componentDidMount(
    (props) => {
    }
)(TopPanel);

export default connect(
    (state) => {
        return state;
    }
)(TopPanel);


